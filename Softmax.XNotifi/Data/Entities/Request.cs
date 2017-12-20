using System;
using System.ComponentModel.DataAnnotations;
using Softmax.XNotifi.Data.Enums;

namespace Softmax.XNotifi.Data.Entities
{
    public class Request
    {  
        [Key]
        public string RequestId { get; set; }
        public string ApplicationId { get; set; }
        public string GatewayId { get; set; }
        public int Recipients { get; set; } = 0;
        public decimal Cost { get; set; } = 0.00m;
        public string Response { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Application Application { get; set; }
        public virtual Gateway Gateway { get; set; }
    }
}
