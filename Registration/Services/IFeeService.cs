using Registration.DTOs.Fees;

namespace Registration.Services
{
    public interface IFeeService
    {
        Task<FeeResponse> CreateAsync(CreateFeeRequest request);

        Task<List<FeeResponse>> GetByStudentIdAsync(
            int studentId);
    }
}