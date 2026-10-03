using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Students;
using Registration.Models;

namespace Registration.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public StudentService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        // =========================
        // GET ALL STUDENTS
        // =========================

        public async Task<List<StudentResponse>> GetAllAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .Select(s => new StudentResponse
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    CourseId = s.CourseId,
                    ProfileImage = s.ProfileImage
                })
                .ToListAsync();
        }


        // =========================
        // GET STUDENT BY ID
        // =========================

        public async Task<StudentResponse?> GetByIdAsync(int id)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new StudentResponse
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    CourseId = s.CourseId,
                    ProfileImage = s.ProfileImage
                })
                .FirstOrDefaultAsync();
        }


        // =========================
        // GET STUDENT BY USER ID
        // =========================

        public async Task<StudentResponse?> GetByUserIdAsync(int userId)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => new StudentResponse
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    CourseId = s.CourseId,
                    ProfileImage = s.ProfileImage
                })
                .FirstOrDefaultAsync();
        }


        // =========================
        // CREATE STUDENT
        // =========================

        public async Task<StudentResponse> CreateAsync(
            CreateStudentRequest request)
        {
            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email.ToLower() ==
                    request.Email.ToLower());

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }


            var courseExists = await _context.Courses
                .AnyAsync(c =>
                    c.Id == request.CourseId);

            if (!courseExists)
            {
                throw new KeyNotFoundException(
                    "Course not found.");
            }


            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // -------------------------
                // CREATE USER
                // -------------------------

                var user = new User
                {
                    Name = request.Name.Trim(),
                    Email = request.Email.Trim(),
                    Role = UserRole.STUDENT
                };


                user.Password =
                    _passwordHasher.HashPassword(
                        user,
                        request.Password);


                _context.Users.Add(user);

                await _context.SaveChangesAsync();


                // -------------------------
                // CREATE STUDENT
                // -------------------------

                var student = new Student
                {
                    UserId = user.Id,

                    StudentId =
                        $"TEMP-{Guid.NewGuid():N}",

                    Name =
                        request.Name.Trim(),

                    Email =
                        request.Email.Trim(),

                    Phone =
                        request.Phone.Trim(),

                    Address =
                        request.Address.Trim(),

                    CourseId =
                        request.CourseId,

                    ProfileImage = null
                };


                _context.Students.Add(student);

                await _context.SaveChangesAsync();


                // -------------------------
                // GENERATE STUDENT ID
                // -------------------------

                student.StudentId =
                    $"STU{student.Id:D3}";


                await _context.SaveChangesAsync();


                await transaction.CommitAsync();


                return new StudentResponse
                {
                    Id =
                        student.Id,

                    UserId =
                        student.UserId,

                    StudentId =
                        student.StudentId,

                    Name =
                        student.Name,

                    Email =
                        student.Email,

                    Phone =
                        student.Phone,

                    Address =
                        student.Address,

                    CourseId =
                        student.CourseId,

                    ProfileImage =
                        student.ProfileImage
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // =========================
        // UPDATE STUDENT
        // =========================

        public async Task<StudentResponse?> UpdateAsync(
            int id,
            UpdateStudentRequest request)
        {
            var student =
                await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(
                        s => s.Id == id);


            if (student == null)
                return null;


            // -------------------------
            // EMAIL DUPLICATE CHECK
            // -------------------------

            var emailExists =
                await _context.Users
                    .AnyAsync(u =>
                        u.Email.ToLower() ==
                        request.Email.ToLower() &&
                        u.Id != student.UserId);


            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }


            // -------------------------
            // COURSE CHECK
            // -------------------------

            var courseExists =
                await _context.Courses
                    .AnyAsync(
                        c => c.Id == request.CourseId);


            if (!courseExists)
            {
                throw new KeyNotFoundException(
                    "Course not found.");
            }


            // StudentId is intentionally
            // NOT changed.

            student.Name =
                request.Name.Trim();

            student.Email =
                request.Email.Trim();

            student.Phone =
                request.Phone.Trim();

            student.Address =
                request.Address.Trim();

            student.CourseId =
                request.CourseId;


            // -------------------------
            // PROFILE IMAGE
            // -------------------------

            // If the request contains a new image,
            // save it.
            //
            // If it is null, keep the existing image.

            if (!string.IsNullOrWhiteSpace(
                request.ProfileImage))
            {
                student.ProfileImage =
                    request.ProfileImage;
            }


            // -------------------------
            // UPDATE USER TABLE
            // -------------------------

            if (student.User != null)
            {
                student.User.Name =
                    request.Name.Trim();

                student.User.Email =
                    request.Email.Trim();
            }


            await _context.SaveChangesAsync();


            return new StudentResponse
            {
                Id =
                    student.Id,

                UserId =
                    student.UserId,

                StudentId =
                    student.StudentId,

                Name =
                    student.Name,

                Email =
                    student.Email,

                Phone =
                    student.Phone,

                Address =
                    student.Address,

                CourseId =
                    student.CourseId,

                ProfileImage =
                    student.ProfileImage
            };
        }


        // =========================
        // DELETE STUDENT
        // =========================

        public async Task<bool> DeleteAsync(int id)
        {
            var student =
                await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(
                        s => s.Id == id);


            if (student == null)
                return false;


            _context.Students.Remove(student);


            if (student.User != null)
            {
                _context.Users.Remove(student.User);
            }


            await _context.SaveChangesAsync();


            return true;
        }
    }
}