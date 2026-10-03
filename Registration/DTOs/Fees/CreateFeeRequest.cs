using System.ComponentModel.DataAnnotations;

namespace Registration.DTOs.Fees
{
    public class CreateFeeRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}