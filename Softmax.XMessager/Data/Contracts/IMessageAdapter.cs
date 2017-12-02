using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts
{
    public interface IMessageAdapter
    {
        string Send(MessageRequestModel message, GatewayModel gateway);
    }
}
