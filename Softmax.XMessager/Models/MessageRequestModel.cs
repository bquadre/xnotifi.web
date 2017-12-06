using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XMessager.Data.Enums;

namespace Softmax.XMessager.Models
{
    public class MessageRequestModel
    {
        [Required]
        public string AppId { get; set; }

        [Required]
        public string AppKey { get; set; }

        [Required]
        public ServiceCode Service { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Message { get; set; }

        public string Subject { get; set; }
        public string Brand { get; set; }
    }
}
