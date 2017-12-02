using System;
//using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Data.Enums;

namespace Softmax.XMessager.Models
{
    public class RequestModel
    {
        public string RequestId { get; set; }
        public string ApplicationId { get; set; }
        public string GatewayId { get; set; }
        public ServiceCode Service { get; set; }
        public int Recipients { get; set; }
        public decimal Cost { get; set; }
        public string Response { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ApplicationModel Application { get; set; }
        public virtual GatewayModel Gateway { get; set; }
    }
}
