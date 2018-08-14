using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Utitities
{
    public class XNotifiSettings
    {
        public string BaseUrl { get; set; }
        public bool Development { get; set; }
        public decimal SmsPrice { get; set; }
        public decimal EmailPrice { get; set; }
        public string ClientId { get; set; }
        public string AccessKey { get; set; }
        public string EmailHeader { get; set; }
        public string Brand { get; set; }
        public string EmailFooter { get; set; }
        public string NoReplyEmail { get; set; }
        public string SupportEmail { get; set; }
        public string TechnicalEmail { get; set; }
        public string PaymentMethods { get; set; }
    }
}
