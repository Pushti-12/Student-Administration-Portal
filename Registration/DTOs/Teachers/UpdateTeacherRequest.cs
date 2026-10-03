using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Teachers
{
    public class UpdateTeacherRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }
    }
}