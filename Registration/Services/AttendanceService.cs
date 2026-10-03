using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Attendance;
using Registration.Models;

namespace Registration.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceResponse> CreateAsync(
            CreateAttendanceRequest request)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == request.StudentId);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == request.CourseId);

            if (!courseExists)
                throw new KeyNotFoundException("Course not found.");

            if (student.CourseId != request.CourseId)
                throw new InvalidOperationException(
                    "Student is not enrolled in this course.");

            var attendance = new Attendance
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                Date = request.Date,
                Status = request.Status
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return new AttendanceResponse
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                CourseId = attendance.CourseId,
                Date = attendance.Date,
                Status = attendance.Status
            };
        }

        public async Task<List<AttendanceResponse>> GetByStudentIdAsync(
            int studentId)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .Select(a => new AttendanceResponse
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    CourseId = a.CourseId,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToListAsync();
        }
    }
}