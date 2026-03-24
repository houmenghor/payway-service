namespace payway.Models
{
    public class PaymentStatusResponse
    {
        public bool IsPaid { get; set; }
        public string Status { get; set; } = null!;
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? TransactionId { get; set; }
    }
}
