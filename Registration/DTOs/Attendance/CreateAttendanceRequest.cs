using System.ComponentModel.DataAnnotations;
using Registration.Models;

namespace Registration.DTOs.Attendance
{
    public class CreateAttendanceRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }
    }
}