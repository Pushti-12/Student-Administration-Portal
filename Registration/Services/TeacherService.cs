using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Teachers;
using Registration.Models;

namespace Registration.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public TeacherService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        // =====================================================
        // GET ALL TEACHERS
        // =====================================================

        public async Task<List<TeacherResponse>> GetAllAsync()
        {
            return await _context.Teachers
                .AsNoTracking()
                .Select(t => new TeacherResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TeacherId = t.TeacherId,
                    Name = t.Name,
                    Email = t.Email,
                    Phone = t.Phone,
                    ProfileImage = t.ProfileImage
                })
                .ToListAsync();
        }


        // =====================================================
        // GET TEACHER BY USER ID
        // Used by logged-in teacher profile
        // =====================================================

        public async Task<TeacherResponse?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .Select(t => new TeacherResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TeacherId = t.TeacherId,
                    Name = t.Name,
                    Email = t.Email,
                    Phone = t.Phone,
                    ProfileImage = t.ProfileImage
                })
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // CREATE TEACHER
        // Admin creates teacher
        // =====================================================

        public async Task<TeacherResponse> CreateAsync(
            CreateTeacherRequest request)
        {
            var emailExists =
                await _context.Users.AnyAsync(
                    u =>
                        u.Email.ToLower() ==
                        request.Email.Trim().ToLower()
                );

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists."
                );
            }


            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // ---------------------------------------------
                // CREATE USER
                // ---------------------------------------------

                var user = new User
                {
                    Name = request.Name.Trim(),
                    Email = request.Email.Trim(),
                    Role = UserRole.TEACHER
                };


                user.Password =
                    _passwordHasher.HashPassword(
                        user,
                        request.Password
                    );


                _context.Users.Add(user);

                await _context.SaveChangesAsync();


                // ---------------------------------------------
                // CREATE TEACHER
                // ---------------------------------------------

                var teacher = new Teacher
                {
                    UserId = user.Id,

                    // Temporary ID because database ID
                    // is generated after SaveChanges.
                    TeacherId =
                        $"TEMP-{Guid.NewGuid():N}",

                    Name = request.Name.Trim(),

                    Email = request.Email.Trim(),

                    Phone = request.Phone.Trim(),

                    ProfileImage = null
                };


                _context.Teachers.Add(teacher);

                await _context.SaveChangesAsync();


                // ---------------------------------------------
                // GENERATE FINAL TEACHER ID
                // ---------------------------------------------

                teacher.TeacherId =
                    $"TEA{teacher.Id:D3}";


                await _context.SaveChangesAsync();


                await transaction.CommitAsync();


                return new TeacherResponse
                {
                    Id = teacher.Id,

                    UserId = teacher.UserId,

                    TeacherId = teacher.TeacherId,

                    Name = teacher.Name,

                    Email = teacher.Email,

                    Phone = teacher.Phone,

                    ProfileImage = teacher.ProfileImage
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }


        // =====================================================
        // UPDATE TEACHER
        //
        // Used by:
        // 1. Admin teacher edit
        // 2. Teacher own profile update
        //
        // ProfileImage is optional.
        // Existing image is preserved when no new image
        // is provided.
        // =====================================================

        public async Task<TeacherResponse?> UpdateAsync(
            int id,
            UpdateTeacherRequest request)
        {
            var teacher =
                await _context.Teachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(
                        t => t.Id == id
                    );


            if (teacher == null)
            {
                return null;
            }


            // ---------------------------------------------
            // CHECK EMAIL DUPLICATE
            // ---------------------------------------------

            var emailExists =
                await _context.Users.AnyAsync(
                    u =>
                        u.Email.ToLower() ==
                        request.Email.Trim().ToLower()
                        &&
                        u.Id != teacher.UserId
                );


            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists."
                );
            }


            // ---------------------------------------------
            // UPDATE TEACHER INFORMATION
            // ---------------------------------------------

            teacher.Name =
                request.Name.Trim();

            teacher.Email =
                request.Email.Trim();

            teacher.Phone =
                request.Phone.Trim();


            // ---------------------------------------------
            // UPDATE PROFILE IMAGE
            //
            // IMPORTANT:
            // Only update when a new image is actually sent.
            //
            // This prevents Admin's existing Edit Teacher
            // functionality from accidentally removing the
            // teacher's profile image.
            // ---------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                request.ProfileImage))
            {
                teacher.ProfileImage =
                    request.ProfileImage;
            }


            // ---------------------------------------------
            // UPDATE RELATED USER
            // ---------------------------------------------

            if (teacher.User != null)
            {
                teacher.User.Name =
                    request.Name.Trim();

                teacher.User.Email =
                    request.Email.Trim();
            }


            // ---------------------------------------------
            // SAVE DATABASE
            // ---------------------------------------------

            await _context.SaveChangesAsync();


            // ---------------------------------------------
            // RETURN UPDATED TEACHER
            // ---------------------------------------------

            return new TeacherResponse
            {
                Id = teacher.Id,

                UserId = teacher.UserId,

                TeacherId = teacher.TeacherId,

                Name = teacher.Name,

                Email = teacher.Email,

                Phone = teacher.Phone,

                ProfileImage = teacher.ProfileImage
            };
        }


        // =====================================================
        // DELETE TEACHER
        // =====================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher =
                await _context.Teachers
                    .Include(t => t.User)
                    .Include(t => t.Courses)
                    .FirstOrDefaultAsync(
                        t => t.Id == id
                    );


            if (teacher == null)
            {
                return false;
            }


            // ---------------------------------------------
            // DO NOT DELETE TEACHER IF COURSES ARE ASSIGNED
            // ---------------------------------------------

            if (teacher.Courses.Any())
            {
                throw new InvalidOperationException(
                    "Teacher cannot be deleted while courses are assigned."
                );
            }


            // ---------------------------------------------
            // DELETE TEACHER
            // ---------------------------------------------

            _context.Teachers.Remove(teacher);


            // ---------------------------------------------
            // DELETE RELATED USER
            // ---------------------------------------------

            if (teacher.User != null)
            {
                _context.Users.Remove(teacher.User);
            }


            await _context.SaveChangesAsync();


            return true;
        }
    }
}