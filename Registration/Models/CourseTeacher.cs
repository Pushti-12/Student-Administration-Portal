using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class CourseTeacher
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        public int TeacherId { get; set; }

        public Course? Course { get; set; }

        public Teacher? Teacher { get; set; }
    }
}