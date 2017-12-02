using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XMessager.Data.Enums;

namespace Softmax.XMessager.Models
{
    public class GatewayModel
    {
        public string GatewayId { get; set; }
        [Required, Display(Name="Service")]
        public ServiceCode ServiceCode { get; set; }

        [Required, Display(Name = "Service Url")]
        public string ServiceUrl { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required, Display(Name = "Provider Url")]
        public string ProviderUrl { get; set; }

        [Required, Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Use SSL")]
        public bool IsSecure { get; set; }
        public int Port { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
