﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts
{
    public interface IMessageAdapter
    {
        string Send(MessageRequestModel message, GatewayModel gateway);
    }
}
