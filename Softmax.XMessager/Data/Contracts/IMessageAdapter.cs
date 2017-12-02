using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts
{
    public interface IMessageAdapter
    {
        string SmsService(MessageRequestModel message, GatewayModel gateway);
        string Emailservice(MessageRequestModel message, GatewayModel gateway);
    }
}
