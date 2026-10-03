using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Marks
{
    public class CreateMarksRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public string ExamName { get; set; } = string.Empty;

        [Range(0, 100)]
        public int MarksObtained { get; set; }
    }
}