using System.ComponentModel.DataAnnotations;

namespace Registration.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword",
            ErrorMessage = "New passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}