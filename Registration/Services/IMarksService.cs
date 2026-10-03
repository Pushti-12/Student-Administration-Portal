using Registration.DTOs.Marks;

namespace Registration.Services
{
    public interface IMarksService
    {
        Task<MarksResponse> CreateAsync(CreateMarksRequest request);

        Task<List<MarksResponse>> GetByStudentIdAsync(
            int studentId);

        Task DeleteAsync(int id);

    }
}