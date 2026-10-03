using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string TeacherId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string Phone { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public User? User { get; set; }

        // Existing primary relationship
        public ICollection<Course> Courses { get; set; }
            = new List<Course>();

        // Additional/shared course assignments
        public ICollection<CourseTeacher> CourseTeachers { get; set; }
            = new List<CourseTeacher>();
    }
}