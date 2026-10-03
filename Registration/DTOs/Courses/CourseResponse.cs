using Registration.DTOs.Subjects;

namespace Registration.DTOs.Courses
{
    public class CourseResponse
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string CourseCode { get; set; } = string.Empty;

        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public List<int> TeacherIds { get; set; }
            = new();

        public List<string> TeacherNames { get; set; }
            = new();

        public List<SubjectResponse> Subjects { get; set; }
            = new();
    }
}