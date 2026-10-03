using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Marks
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        // Nullable temporarily so existing marks
        // can remain valid during migration.
        public int? SubjectId { get; set; }

        [Required]
        public string ExamName { get; set; } = string.Empty;

        public int MarksObtained { get; set; }

        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        public Student? Student { get; set; }

        public Course? Course { get; set; }

        public Subject? Subject { get; set; }
    }
}