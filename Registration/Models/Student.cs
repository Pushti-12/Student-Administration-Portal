using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the phone number")]
        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public int CourseId { get; set; }

        // Profile image is stored as a Base64 data URL.
        // It is optional so existing students remain valid.
        public string? ProfileImage { get; set; }

        // Navigation Properties
        public User? User { get; set; }

        public Course? Course { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
            = new List<Attendance>();

        public ICollection<Marks> Marks { get; set; }
            = new List<Marks>();

        public ICollection<Fee> Fees { get; set; }
            = new List<Fee>();
    }
}