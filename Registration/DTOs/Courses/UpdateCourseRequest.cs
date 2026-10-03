using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Courses
{
    public class UpdateCourseRequest
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        public int TeacherId { get; set; }
    }
}