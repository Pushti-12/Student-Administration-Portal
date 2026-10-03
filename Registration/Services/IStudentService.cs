using Registration.DTOs.Students;

namespace Registration.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponse>> GetAllAsync();

        Task<StudentResponse?> GetByIdAsync(int id);

        Task<StudentResponse?> GetByUserIdAsync(int userId);

        Task<StudentResponse> CreateAsync(
            CreateStudentRequest request);

        Task<StudentResponse?> UpdateAsync(
            int id,
            UpdateStudentRequest request);

        Task<bool> DeleteAsync(int id);
    }
}