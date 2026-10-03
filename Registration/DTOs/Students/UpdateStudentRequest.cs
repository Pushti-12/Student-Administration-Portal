using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Students
{
    public class UpdateStudentRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        public string? ProfileImage { get; set; }
    }
}