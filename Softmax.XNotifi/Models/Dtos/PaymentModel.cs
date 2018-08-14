using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models
{
    public class PaymentModel
    {
        public string PaymentId { get; set; }
        public string ClientId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
