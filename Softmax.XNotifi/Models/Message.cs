using Softmax.XNotifi.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models
{
    public class Message
    {
        [Key]
        public string MessageId { get; set; }

        public MessageType MessageType { get; set; }

        public MessageMethod MessageMethod { get; set; }

        public string AspNetUsersId { get; set; }

        public string Text { get; set; }

        public string Sender { get; set; }

        public string Recipients { get; set; }

        public int Count { get; set; }

        public int Cost { get; set; }

        public string GatewayResponse { get; set; }

        public StatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }


    }
}
