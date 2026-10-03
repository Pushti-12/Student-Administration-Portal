using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Admin;
using Registration.Models;

namespace Registration.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;


        public AdminService(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET CURRENT ADMIN
        // =========================================

        public async Task<AdminResponse?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == userId &&
                    u.Role == UserRole.ADMIN)
                .Select(u => new AdminResponse
                {
                    Id =
                        u.Id,

                    Name =
                        u.Name,

                    Email =
                        u.Email,

                    Phone =
                        u.Phone ?? string.Empty,

                    Role =
                        u.Role.ToString(),

                    ProfileImage =
                        u.ProfileImage
                })
                .FirstOrDefaultAsync();
        }


        // =========================================
        // UPDATE CURRENT ADMIN
        // =========================================

        public async Task<AdminResponse?> UpdateAsync(
            int userId,
            UpdateAdminRequest request)
        {
            var admin =
                await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == userId &&
                        u.Role == UserRole.ADMIN);


            if (admin == null)
            {
                return null;
            }


            // =========================================
            // DUPLICATE EMAIL CHECK
            // =========================================

            var emailExists =
                await _context.Users
                    .AnyAsync(u =>
                        u.Email.ToLower() ==
                        request.Email.Trim().ToLower() &&
                        u.Id != userId);


            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }


            // =========================================
            // UPDATE USER DETAILS
            // =========================================

            admin.Name =
                request.Name.Trim();

            admin.Email =
                request.Email.Trim();

            admin.Phone =
                request.Phone.Trim();


            // =========================================
            // UPDATE PROFILE IMAGE
            // =========================================

            // If a new image was selected,
            // replace the old image.

            // If no new image was selected,
            // keep the existing image.

            if (!string.IsNullOrWhiteSpace(
                request.ProfileImage))
            {
                admin.ProfileImage =
                    request.ProfileImage;
            }


            await _context.SaveChangesAsync();


            // =========================================
            // RETURN UPDATED ADMIN
            // =========================================

            return new AdminResponse
            {
                Id =
                    admin.Id,

                Name =
                    admin.Name,

                Email =
                    admin.Email,

                Phone =
                    admin.Phone ?? string.Empty,

                Role =
                    admin.Role.ToString(),

                ProfileImage =
                    admin.ProfileImage
            };
        }
    }
}