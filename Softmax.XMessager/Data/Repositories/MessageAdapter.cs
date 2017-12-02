using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Repositories
{
    public class MessagerAdapter : IMessageAdapter
    {
        public string Send(MessageRequestModel message, GatewayModel gateway)
        {
            throw new NotImplementedException();
        }
    }
}
