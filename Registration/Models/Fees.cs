using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Fee
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        // Navigation Property
        public Student? Student { get; set; }
    }
}