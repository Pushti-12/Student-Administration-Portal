using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        // Navigation
        public Course? Course { get; set; }

        public ICollection<Marks> Marks { get; set; }
            = new List<Marks>();
    }
}