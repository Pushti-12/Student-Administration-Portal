namespace Registration.DTOs.Teachers
{
    public class TeacherResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }
    }

}