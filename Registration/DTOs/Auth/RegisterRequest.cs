using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Auth
{
    public class RegisterRequest
    {
        // =========================
        // COMMON
        // =========================

        [Required(ErrorMessage = "Enter your Name")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(ADMIN|TEACHER|STUDENT)$",
            ErrorMessage = "Role must be ADMIN, TEACHER, or STUDENT")]
        public string Role { get; set; } = string.Empty;


        // =========================
        // STUDENT
        // ONE COURSE ONLY
        // =========================

        public int? CourseId { get; set; }


        // =========================
        // TEACHER
        // EXISTING COURSES
        // =========================

        public List<int> CourseIds { get; set; }
            = new List<int>();


        // =========================
        // TEACHER
        // NEW COURSES
        // =========================

        public List<NewCourseRequest> NewCourses { get; set; }
            = new List<NewCourseRequest>();


        // =========================
        // CONTACT
        // =========================

        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? Phone { get; set; }


        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string? Address { get; set; }
    }
}