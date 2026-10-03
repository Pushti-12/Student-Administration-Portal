using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Auth
{
    public class NewCourseRequest
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;
    }
}