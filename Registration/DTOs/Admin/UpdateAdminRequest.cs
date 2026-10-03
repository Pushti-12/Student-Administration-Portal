using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Admin
{
    public class UpdateAdminRequest
    {
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


        public string? ProfileImage { get; set; }
    }
}