using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace payway.Models
{
    public class PaymentCallbackRequest
    {
        public string BillNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty; // e.g., "PAID"
        public string MD5 { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }
}