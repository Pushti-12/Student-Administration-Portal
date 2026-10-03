using Registration.DTOs.Teachers;

namespace Registration.Services
{
    public interface ITeacherService
    {
        Task<List<TeacherResponse>> GetAllAsync();

        Task<TeacherResponse?> GetByUserIdAsync(int userId);

        Task<TeacherResponse> CreateAsync(
            CreateTeacherRequest request);

        Task<TeacherResponse?> UpdateAsync(
            int id,
            UpdateTeacherRequest request);

        Task<bool> DeleteAsync(int id);
    }
}