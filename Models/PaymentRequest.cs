namespace payway.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string? BillNumber { get; set; }
        public string? MobileNumber { get; set; }
    }
}
