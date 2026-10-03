using Registration.Models;

namespace Registration.DTOs.Attendance
{
    public class AttendanceResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}