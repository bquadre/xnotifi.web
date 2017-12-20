using System;
using System.ComponentModel.DataAnnotations;
using Softmax.XNotifi.Data.Enums;

namespace Softmax.XNotifi.Data.Entities
{
    public class Gateway
    {
        [Key]
        public string GatewayId { get; set; }
        public ServiceCode ServiceCode { get; set; }
        public string ServiceUrl { get; set; }
        public string Provider { get; set; }
        public string ProviderUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
