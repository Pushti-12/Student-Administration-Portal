namespace Registration.DTOs.Subjects
{
    public class SubjectResponse
    {
        public int Id { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;
    }
}