using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Subjects
{
    public class CreateSubjectRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(150)]
        public string SubjectName { get; set; } = string.Empty;
    }
}