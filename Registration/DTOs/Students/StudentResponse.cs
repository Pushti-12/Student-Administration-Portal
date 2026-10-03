namespace Registration.DTOs.Students
{
    public class StudentResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int CourseId { get; set; }

        public string? ProfileImage { get; set; }
    }
}