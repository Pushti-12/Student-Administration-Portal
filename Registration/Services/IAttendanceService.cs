using Registration.DTOs.Attendance;

namespace Registration.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceResponse> CreateAsync(
            CreateAttendanceRequest request);

        Task<List<AttendanceResponse>> GetByStudentIdAsync(
            int studentId);
    }
}
