using Registration.DTOs.Admin;

namespace Registration.Services
{
    public interface IAdminService
    {
        Task<AdminResponse?> GetByUserIdAsync(int userId);

        Task<AdminResponse?> UpdateAsync(
            int userId,
            UpdateAdminRequest request);
    }
}