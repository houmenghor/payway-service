using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace payway
{
    public class BakongSettings
    {
        public string Token { get; set; } = null!;
        public string BakongAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string MerchantName { get; set; } = null!;
        public string MerchantCity { get; set; } = null!;
        public string? InquiryUrl { get; set; }

    }
}