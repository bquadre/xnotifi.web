using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts
{
    public interface IMessageAdapter
    {
        string SmsService(MessageRequestModel message, GatewayModel gateway);
        string Emailservice(MessageRequestModel message, GatewayModel gateway);
    }
}
