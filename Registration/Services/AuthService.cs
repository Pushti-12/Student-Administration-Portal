using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Registration.Data;
using Registration.DTOs.Auth;
using Registration.Models;

namespace Registration.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;


        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IPasswordHasher<User> passwordHasher,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }


        // =========================
        // LOGIN
        // =========================

        public async Task<LoginResponse?> LoginAsync(
            LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() ==
                    request.Email.Trim().ToLower());

            if (user == null)
                return null;


            var result =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.Password,
                    request.Password);


            if (result ==
                PasswordVerificationResult.Failed)
            {
                return null;
            }


            return new LoginResponse
            {
                Token =
                    GenerateJwtToken(user),

                Name =
                    user.Name,

                Email =
                    user.Email,

                Role =
                    user.Role.ToString()
            };
        }


        // =========================
        // REGISTER
        // =========================

        public async Task<bool> RegisterAsync(
            RegisterRequest request)
        {
            if (!Enum.TryParse<UserRole>(
                    request.Role,
                    true,
                    out var role))
            {
                return false;
            }


            if (role == UserRole.ADMIN)
                return false;


            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return false;
            }


            var emailExists =
                await _context.Users.AnyAsync(u =>
                    u.Email.ToLower() ==
                    request.Email.Trim().ToLower());


            if (emailExists)
                return false;


            // =========================
            // STUDENT
            // =========================

            if (role == UserRole.STUDENT)
            {
                if (!request.CourseId.HasValue)
                    return false;


                if (string.IsNullOrWhiteSpace(request.Phone) ||
                    string.IsNullOrWhiteSpace(request.Address))
                {
                    return false;
                }


                var courseExists =
                    await _context.Courses.AnyAsync(
                        c => c.Id ==
                             request.CourseId.Value);


                if (!courseExists)
                    return false;
            }


            // =========================
            // TEACHER
            // =========================

            if (role == UserRole.TEACHER)
            {
                if (string.IsNullOrWhiteSpace(request.Phone))
                    return false;


                request.CourseIds =
                    (request.CourseIds ?? new List<int>())
                        .Distinct()
                        .ToList();


                request.NewCourses =
                    (request.NewCourses ??
                     new List<NewCourseRequest>())
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(
                            c.CourseName) &&
                        !string.IsNullOrWhiteSpace(
                            c.CourseCode))
                    .ToList();


                // At least one existing
                // OR new course.
                if (request.CourseIds.Count == 0 &&
                    request.NewCourses.Count == 0)
                {
                    return false;
                }


                // Existing course IDs must exist.
                if (request.CourseIds.Count > 0)
                {
                    var existingCourseCount =
                        await _context.Courses
                            .CountAsync(c =>
                                request.CourseIds
                                    .Contains(c.Id));


                    if (existingCourseCount !=
                        request.CourseIds.Count)
                    {
                        return false;
                    }
                }


                // Duplicate course codes
                // are not allowed.
                var newCodes =
                    request.NewCourses
                        .Select(c =>
                            c.CourseCode.Trim()
                                .ToLower())
                        .ToList();


                if (newCodes.Count !=
                    newCodes.Distinct().Count())
                {
                    return false;
                }


                foreach (
                    var newCourse
                    in request.NewCourses)
                {
                    var codeExists =
                        await _context.Courses
                            .AnyAsync(c =>
                                c.CourseCode.ToLower() ==
                                newCourse.CourseCode
                                    .Trim()
                                    .ToLower());


                    if (codeExists)
                        return false;
                }
            }


            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =========================
                // CREATE USER
                // =========================

                var user = new User
                {
                    Name =
                        request.Name.Trim(),

                    Email =
                        request.Email.Trim(),

                    Role =
                        role
                };


                user.Password =
                    _passwordHasher.HashPassword(
                        user,
                        request.Password);


                _context.Users.Add(user);

                await _context.SaveChangesAsync();


                // =========================
                // CREATE STUDENT
                // =========================

                if (role == UserRole.STUDENT)
                {
                    var student = new Student
                    {
                        UserId =
                            user.Id,

                        StudentId =
                            $"TEMP-{Guid.NewGuid():N}",

                        Name =
                            request.Name.Trim(),

                        Email =
                            request.Email.Trim(),

                        Phone =
                            request.Phone!.Trim(),

                        Address =
                            request.Address!.Trim(),

                        CourseId =
                            request.CourseId!.Value
                    };


                    _context.Students.Add(student);

                    await _context.SaveChangesAsync();


                    student.StudentId =
                        $"STU{student.Id:D3}";

                    await _context.SaveChangesAsync();
                }


                // =========================
                // CREATE TEACHER
                // =========================

                if (role == UserRole.TEACHER)
                {
                    var teacher = new Teacher
                    {
                        UserId =
                            user.Id,

                        TeacherId =
                            $"TEMP-{Guid.NewGuid():N}",

                        Name =
                            request.Name.Trim(),

                        Email =
                            request.Email.Trim(),

                        Phone =
                            request.Phone!.Trim()
                    };


                    _context.Teachers.Add(teacher);

                    await _context.SaveChangesAsync();


                    teacher.TeacherId =
                        $"TEA{teacher.Id:D3}";

                    await _context.SaveChangesAsync();


                    // =========================
                    // EXISTING COURSES
                    // =========================

                    foreach (
                        var courseId
                        in request.CourseIds)
                    {
                        var course =
                            await _context.Courses
                                .FirstAsync(c =>
                                    c.Id == courseId);


                        // Keep old primary teacher.
                        // Only assign additional teacher.

                        var assignmentExists =
                            await _context.CourseTeachers
                                .AnyAsync(ct =>
                                    ct.CourseId ==
                                        course.Id
                                    &&
                                    ct.TeacherId ==
                                        teacher.Id);


                        if (!assignmentExists)
                        {
                            _context.CourseTeachers.Add(
                                new CourseTeacher
                                {
                                    CourseId =
                                        course.Id,

                                    TeacherId =
                                        teacher.Id
                                });
                        }
                    }


                    // =========================
                    // NEW COURSES
                    // =========================

                    foreach (
                        var newCourse
                        in request.NewCourses)
                    {
                        var course =
                            new Course
                            {
                                CourseName =
                                    newCourse.CourseName
                                        .Trim(),

                                CourseCode =
                                    newCourse.CourseCode
                                        .Trim(),

                                // New course's
                                // primary teacher
                                // is this teacher.
                                TeacherId =
                                    teacher.Id
                            };


                        _context.Courses.Add(course);

                        await _context.SaveChangesAsync();


                        _context.CourseTeachers.Add(
                            new CourseTeacher
                            {
                                CourseId =
                                    course.Id,

                                TeacherId =
                                    teacher.Id
                            });
                    }


                    await _context.SaveChangesAsync();
                }


                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();

                return false;
            }
        }


        // =========================
        // FORGOT PASSWORD
        // =========================

        public async Task<bool> ForgotPasswordAsync(
            ForgotPasswordRequest request,
            string resetLink)
        {
            var email =
                request.Email.Trim().ToLower();


            var user =
                await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email.ToLower() ==
                        email);


            if (user == null)
            {
                return false;
            }


            // Generate secure random token
            var tokenBytes =
                RandomNumberGenerator.GetBytes(32);


            var resetToken =
                Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");


            // Save token in database
            user.ResetToken =
                resetToken;


            // Token valid for 30 minutes
            user.ResetTokenExpiry =
                DateTime.UtcNow.AddMinutes(30);


            await _context.SaveChangesAsync();


            // Add token to reset URL
            var finalResetLink =
                $"{resetLink}?token={Uri.EscapeDataString(resetToken)}";


            // Send email
            await _emailService
                .SendPasswordResetEmailAsync(
                    user.Email,
                    finalResetLink);


            return true;
        }

        // =========================
        // RESET PASSWORD
        // =========================

        public async Task<bool> ResetPasswordAsync(
            ResetPasswordRequest request)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.ResetToken == request.Token);


            // Token doesn't exist
            if (user == null)
            {
                return false;
            }


            // Token has expired
            if (!user.ResetTokenExpiry.HasValue ||
                user.ResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return false;
            }


            // Hash the new password
            user.Password =
                _passwordHasher.HashPassword(
                    user,
                    request.NewPassword);


            // Token can be used only once
            user.ResetToken = null;

            user.ResetTokenExpiry = null;


            await _context.SaveChangesAsync();


            return true;
        }


        // =========================
        // LOGOUT
        // =========================

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }


        // =========================
        // JWT
        // =========================

        private string GenerateJwtToken(
            User user)
        {
            var jwtKey =
                _configuration["Jwt:Key"];


            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT key is not configured.");
            }


            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        user.Name),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email),

                    new Claim(
                        ClaimTypes.Role,
                        user.Role.ToString())
                };


            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);


            var token =
                new JwtSecurityToken(
                    claims: claims,

                    expires:
                        DateTime.UtcNow
                            .AddHours(2),

                    signingCredentials:
                        credentials);


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}