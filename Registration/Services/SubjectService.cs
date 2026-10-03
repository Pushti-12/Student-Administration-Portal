using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Subjects;
using Registration.Models;

namespace Registration.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;

        public SubjectService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectResponse>>
            GetByCourseIdAsync(int courseId)
        {
            return await _context.Subjects
                .AsNoTracking()
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.SubjectName)
                .Select(s => new SubjectResponse
                {
                    Id = s.Id,
                    SubjectName = s.SubjectName,
                    CourseId = s.CourseId,
                    CourseName = s.Course!.CourseName
                })
                .ToListAsync();
        }

        public async Task<SubjectResponse>
            CreateAsync(
                CreateSubjectRequest request)
        {
            var name =
                request.SubjectName.Trim();

            var course =
                await _context.Courses
                    .FirstOrDefaultAsync(
                        c => c.Id == request.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException(
                    "Course not found.");
            }

            var duplicate =
                await _context.Subjects.AnyAsync(
                    s =>
                        s.CourseId == request.CourseId &&
                        s.SubjectName.ToLower() ==
                        name.ToLower());

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Subject already exists in this course.");
            }

            var subject = new Subject
            {
                SubjectName = name,
                CourseId = request.CourseId
            };

            _context.Subjects.Add(subject);

            await _context.SaveChangesAsync();

            return new SubjectResponse
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                CourseId = subject.CourseId,
                CourseName = course.CourseName
            };
        }

        public async Task<SubjectResponse?>
            UpdateAsync(
                int id,
                CreateSubjectRequest request)
        {
            var subject =
                await _context.Subjects
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(
                        s => s.Id == id);

            if (subject == null)
                return null;

            var name =
                request.SubjectName.Trim();

            var course =
                await _context.Courses
                    .FirstOrDefaultAsync(
                        c => c.Id == request.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException(
                    "Course not found.");
            }

            var duplicate =
                await _context.Subjects.AnyAsync(
                    s =>
                        s.Id != id &&
                        s.CourseId == request.CourseId &&
                        s.SubjectName.ToLower() ==
                        name.ToLower());

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Subject already exists in this course.");
            }

            subject.SubjectName = name;
            subject.CourseId = request.CourseId;

            await _context.SaveChangesAsync();

            return new SubjectResponse
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                CourseId = subject.CourseId,
                CourseName = course.CourseName
            };
        }

        public async Task<bool>
            DeleteAsync(int id)
        {
            var subject =
                await _context.Subjects
                    .FirstOrDefaultAsync(
                        s => s.Id == id);

            if (subject == null)
                return false;

            var hasMarks =
                await _context.Marks.AnyAsync(
                    m => m.SubjectId == id);

            if (hasMarks)
            {
                throw new InvalidOperationException(
                    "Subject cannot be deleted because marks already exist for it.");
            }

            _context.Subjects.Remove(subject);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}