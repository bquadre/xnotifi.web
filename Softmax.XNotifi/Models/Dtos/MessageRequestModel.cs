using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XNotifi.Data.Enums;

namespace Softmax.XNotifi.Models
{
    public class MessageRequestModel
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string AccesssKey { get; set; }

        [Required]
        public string AppName { get; set; }

        [Required]
        public ServiceCode Service { get; set; }

        [Required]
        public string From { get; set; } //email or phone numbers

        [Required]
        public string To { get; set; }

        [Required]
        public string Message { get; set; }

        public string Subject { get; set; }
        public string Brand { get; set; }
    }
}
