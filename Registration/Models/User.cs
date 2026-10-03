using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }


        // =========================
        // PROFILE
        // =========================

        // Nullable because existing users
        // may not have a phone number yet.
        public string? Phone { get; set; }

        // Nullable because existing users
        // may not have a profile image yet.
        public string? ProfileImage { get; set; }


        // =========================
        // PASSWORD RESET
        // =========================

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }


        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        public Student? Student { get; set; }

        public Teacher? Teacher { get; set; }
    }


    public enum UserRole
    {
        ADMIN,
        TEACHER,
        STUDENT
    }
}