namespace Registration.DTOs.Fees
{
    public class FeeResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}