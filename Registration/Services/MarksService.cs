using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Marks;

namespace Registration.Services
{
    public class MarksService : IMarksService
    {
        private readonly ApplicationDbContext _context;

        public MarksService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MarksResponse>
            CreateAsync(
                CreateMarksRequest request)
        {
            var student =
                await _context.Students
                    .FirstOrDefaultAsync(
                        s => s.Id == request.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException(
                    "Student not found.");
            }

            if (student.CourseId != request.CourseId)
            {
                throw new InvalidOperationException(
                    "Student is not enrolled in this course.");
            }

            var course =
                await _context.Courses
                    .FirstOrDefaultAsync(
                        c => c.Id == request.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException(
                    "Course not found.");
            }

            var subject =
                await _context.Subjects
                    .FirstOrDefaultAsync(
                        s =>
                            s.Id == request.SubjectId &&
                            s.CourseId == request.CourseId);

            if (subject == null)
            {
                throw new InvalidOperationException(
                    "Selected subject does not belong to the selected course.");
            }

            var marks = new Models.Marks
            {
                StudentId =
                    request.StudentId,

                CourseId =
                    request.CourseId,

                SubjectId =
                    request.SubjectId,

                ExamName =
                    request.ExamName.Trim(),

                MarksObtained =
                    request.MarksObtained
            };

            _context.Marks.Add(marks);

            await _context.SaveChangesAsync();

            return new MarksResponse
            {
                Id =
                    marks.Id,

                StudentId =
                    marks.StudentId,

                CourseId =
                    marks.CourseId,

                SubjectId =
                    marks.SubjectId,

                CourseName =
                    course.CourseName,

                SubjectName =
                    subject.SubjectName,

                ExamName =
                    marks.ExamName,

                MarksObtained =
                    marks.MarksObtained
            };
        }

        public async Task<List<MarksResponse>>
            GetByStudentIdAsync(
                int studentId)
        {
            return await _context.Marks
                .AsNoTracking()
                .Include(m => m.Course)
                .Include(m => m.Subject)
                .Where(m =>
                    m.StudentId == studentId)
                .OrderBy(m => m.Course!.CourseName)
                .ThenBy(m => m.ExamName)
                .ThenBy(m => m.Subject!.SubjectName)
                .Select(m => new MarksResponse
                {
                    Id =
                        m.Id,

                    StudentId =
                        m.StudentId,

                    CourseId =
                        m.CourseId,

                    SubjectId =
                        m.SubjectId,

                    CourseName =
                        m.Course!.CourseName,

                    SubjectName =
                        m.Subject != null
                            ? m.Subject.SubjectName
                            : "—",

                    ExamName =
                        m.ExamName,

                    MarksObtained =
                        m.MarksObtained
                })
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var marks = await _context.Marks
                .FirstOrDefaultAsync(m => m.Id == id);

            if (marks == null)
            {
                throw new KeyNotFoundException("Marks record not found.");
            }

            _context.Marks.Remove(marks);

            await _context.SaveChangesAsync();
        }
    }
}