namespace Registration.DTOs.Admin
{
    public class AdminResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = "ADMIN";

        public string? ProfileImage { get; set; }
    }
}