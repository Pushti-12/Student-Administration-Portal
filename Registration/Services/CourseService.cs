using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Courses;
using Registration.DTOs.Subjects;
using Registration.Models;

namespace Registration.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==================================================
        // GET ALL COURSES
        // GET /api/courses
        // ==================================================

        public async Task<List<CourseResponse>>
            GetAllAsync()
        {
            var courses =
                await _context.Courses
                    .AsNoTracking()

                    .Include(c => c.Teacher)

                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)

                    .Include(c => c.Subjects)

                    .ToListAsync();


            return courses
                .Select(c =>
                {
                    // ==========================================
                    // PRIMARY + ADDITIONAL TEACHER IDS
                    // ==========================================

                    var teacherIds =
                        new List<int>();


                    if (c.TeacherId > 0)
                    {
                        teacherIds.Add(
                            c.TeacherId);
                    }


                    teacherIds.AddRange(
                        c.CourseTeachers
                            .Select(ct =>
                                ct.TeacherId));


                    teacherIds =
                        teacherIds
                            .Distinct()
                            .ToList();


                    // ==========================================
                    // PRIMARY + ADDITIONAL TEACHER NAMES
                    // ==========================================

                    var teacherNames =
                        new List<string>();


                    if (c.Teacher != null)
                    {
                        teacherNames.Add(
                            c.Teacher.Name);
                    }


                    teacherNames.AddRange(
                        c.CourseTeachers
                            .Where(ct =>
                                ct.Teacher != null)
                            .Select(ct =>
                                ct.Teacher!.Name));


                    teacherNames =
                        teacherNames
                            .Distinct()
                            .ToList();


                    // ==========================================
                    // COURSE RESPONSE
                    // ==========================================

                    return new CourseResponse
                    {
                        Id =
                            c.Id,

                        CourseName =
                            c.CourseName,

                        CourseCode =
                            c.CourseCode,

                        TeacherId =
                            c.TeacherId,

                        TeacherName =
                            c.Teacher?.Name,

                        TeacherIds =
                            teacherIds,

                        TeacherNames =
                            teacherNames,

                        Subjects =
                            c.Subjects
                                .OrderBy(s =>
                                    s.SubjectName)
                                .Select(s =>
                                    new SubjectResponse
                                    {
                                        Id =
                                            s.Id,

                                        SubjectName =
                                            s.SubjectName,

                                        CourseId =
                                            s.CourseId,

                                        CourseName =
                                            c.CourseName
                                    })
                                .ToList()
                    };
                })
                .ToList();
        }


        // ==================================================
        // CREATE COURSE
        // POST /api/courses
        // ADMIN
        // ==================================================

        public async Task<CourseResponse>
            CreateAsync(
                CreateCourseRequest request)
        {
            var courseName =
                request.CourseName.Trim();

            var courseCode =
                request.CourseCode.Trim();


            // ==========================================
            // VALIDATE TEACHER
            // ==========================================

            var teacher =
                await _context.Teachers
                    .FirstOrDefaultAsync(
                        t =>
                            t.Id ==
                            request.TeacherId);


            if (teacher == null)
            {
                throw new KeyNotFoundException(
                    "Teacher not found.");
            }


            // ==========================================
            // CHECK COURSE CODE
            // ==========================================

            var codeExists =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseCode.ToLower() ==
                        courseCode.ToLower());


            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Course code already exists.");
            }


            // ==========================================
            // CREATE COURSE
            // ==========================================

            var course =
                new Course
                {
                    CourseName =
                        courseName,

                    CourseCode =
                        courseCode,

                    TeacherId =
                        request.TeacherId
                };


            _context.Courses.Add(course);

            await _context.SaveChangesAsync();


            // ==========================================
            // ADD PRIMARY TEACHER TO ASSIGNMENT TABLE
            // ==========================================

            _context.CourseTeachers.Add(
                new CourseTeacher
                {
                    CourseId =
                        course.Id,

                    TeacherId =
                        request.TeacherId
                });


            await _context.SaveChangesAsync();


            // ==========================================
            // RESPONSE
            // ==========================================

            return new CourseResponse
            {
                Id =
                    course.Id,

                CourseName =
                    course.CourseName,

                CourseCode =
                    course.CourseCode,

                TeacherId =
                    teacher.Id,

                TeacherName =
                    teacher.Name,

                TeacherIds =
                    new List<int>
                    {
                        teacher.Id
                    },

                TeacherNames =
                    new List<string>
                    {
                        teacher.Name
                    },

                Subjects =
                    new List<SubjectResponse>()
            };
        }


        // ==================================================
        // UPDATE COURSE
        // PUT /api/courses/{id}
        // ADMIN
        // ==================================================

        public async Task<CourseResponse?>
            UpdateAsync(
                int id,
                UpdateCourseRequest request)
        {
            var course =
                await _context.Courses
                    .FirstOrDefaultAsync(
                        c =>
                            c.Id == id);


            if (course == null)
            {
                return null;
            }


            // ==========================================
            // VALIDATE TEACHER
            // ==========================================

            var teacher =
                await _context.Teachers
                    .FirstOrDefaultAsync(
                        t =>
                            t.Id ==
                            request.TeacherId);


            if (teacher == null)
            {
                throw new KeyNotFoundException(
                    "Teacher not found.");
            }


            // ==========================================
            // CHECK DUPLICATE COURSE CODE
            // ==========================================

            var courseCode =
                request.CourseCode.Trim();


            var duplicateCode =
                await _context.Courses
                    .AnyAsync(c =>
                        c.CourseCode.ToLower() ==
                        courseCode.ToLower()
                        &&
                        c.Id != id);


            if (duplicateCode)
            {
                throw new InvalidOperationException(
                    "Course code already exists.");
            }


            // ==========================================
            // UPDATE COURSE
            // ==========================================

            course.CourseName =
                request.CourseName.Trim();

            course.CourseCode =
                courseCode;

            course.TeacherId =
                request.TeacherId;


            // ==========================================
            // KEEP TEACHER ASSIGNMENT
            // ==========================================

            var assignmentExists =
                await _context.CourseTeachers
                    .AnyAsync(ct =>
                        ct.CourseId == id
                        &&
                        ct.TeacherId ==
                            request.TeacherId);


            if (!assignmentExists)
            {
                _context.CourseTeachers.Add(
                    new CourseTeacher
                    {
                        CourseId =
                            id,

                        TeacherId =
                            request.TeacherId
                    });
            }


            await _context.SaveChangesAsync();


            // ==========================================
            // LOAD SUBJECTS
            // ==========================================

            var subjects =
                await _context.Subjects
                    .AsNoTracking()
                    .Where(s =>
                        s.CourseId == id)
                    .OrderBy(s =>
                        s.SubjectName)
                    .Select(s =>
                        new SubjectResponse
                        {
                            Id =
                                s.Id,

                            SubjectName =
                                s.SubjectName,

                            CourseId =
                                s.CourseId,

                            CourseName =
                                course.CourseName
                        })
                    .ToListAsync();


            // ==========================================
            // RESPONSE
            // ==========================================

            return new CourseResponse
            {
                Id =
                    course.Id,

                CourseName =
                    course.CourseName,

                CourseCode =
                    course.CourseCode,

                TeacherId =
                    teacher.Id,

                TeacherName =
                    teacher.Name,

                TeacherIds =
                    await GetTeacherIds(id),

                TeacherNames =
                    await GetTeacherNames(id),

                Subjects =
                    subjects
            };
        }


        // ==================================================
        // GET TEACHER IDS
        // ==================================================

        private async Task<List<int>>
            GetTeacherIds(int courseId)
        {
            var course =
                await _context.Courses
                    .AsNoTracking()
                    .FirstAsync(
                        c =>
                            c.Id == courseId);


            var ids =
                await _context.CourseTeachers
                    .Where(ct =>
                        ct.CourseId ==
                        courseId)
                    .Select(ct =>
                        ct.TeacherId)
                    .ToListAsync();


            // Make sure primary TeacherId
            // is also included.
            ids.Add(
                course.TeacherId);


            return ids
                .Distinct()
                .ToList();
        }


        // ==================================================
        // GET TEACHER NAMES
        // ==================================================

        private async Task<List<string>>
            GetTeacherNames(int courseId)
        {
            var names =
                await _context.CourseTeachers
                    .Where(ct =>
                        ct.CourseId ==
                        courseId)
                    .Select(ct =>
                        ct.Teacher!.Name)
                    .ToListAsync();


            var primaryTeacher =
                await _context.Courses
                    .Where(c =>
                        c.Id == courseId)
                    .Select(c =>
                        c.Teacher!.Name)
                    .FirstOrDefaultAsync();


            if (!string.IsNullOrWhiteSpace(
                    primaryTeacher))
            {
                names.Add(
                    primaryTeacher);
            }


            return names
                .Distinct()
                .ToList();
        }
    }
}