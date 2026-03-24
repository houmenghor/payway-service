using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace payway.Models
{
    public class PaymentResponse
    {
        public string QRToken { get; set; } = "";
        public string MD5 { get; set; } = "";
        public string BillNumber { get; set; } = "";
    }
}