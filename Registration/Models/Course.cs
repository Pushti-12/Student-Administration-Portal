using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;

        // Existing primary teacher relationship
        public int TeacherId { get; set; }

        public Teacher? Teacher { get; set; }

        // Additional teacher assignments
        public ICollection<CourseTeacher> CourseTeachers { get; set; }
            = new List<CourseTeacher>();

        // Student -> single course
        public ICollection<Student> Students { get; set; }
            = new List<Student>();

        public ICollection<Attendance> Attendances { get; set; }
            = new List<Attendance>();

        public ICollection<Marks> Marks { get; set; }
            = new List<Marks>();

        // =========================
        // COURSE -> SUBJECTS
        // =========================

        public ICollection<Subject> Subjects { get; set; }
            = new List<Subject>();
    }
}