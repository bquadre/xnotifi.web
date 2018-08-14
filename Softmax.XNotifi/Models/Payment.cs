using Softmax.XNotifi.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models
{
    public class Payment
    {
        [Key]
        public string PaymentId { get; set; }

        public string AspNetUsersId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string GatewayResponse { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
