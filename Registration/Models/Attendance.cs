using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        // Navigation Properties
        public Student? Student { get; set; }

        public Course? Course { get; set; }
    }

    public enum AttendanceStatus
    {
        PRESENT,
        ABSENT
    }
}