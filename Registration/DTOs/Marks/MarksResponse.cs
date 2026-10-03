namespace Registration.DTOs.Marks
{
    public class MarksResponse
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public int? SubjectId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public string ExamName { get; set; } = string.Empty;

        public int MarksObtained { get; set; }
    }
}