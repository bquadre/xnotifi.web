using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softmax.XNotifi.Data.Enums
{
    public enum StatusCode
    {
        MessageSubmitted =2500,
        InvalidFields =2501,
        InvalidService = 2502,
        InvalidApplication =2503,
        InValidClient =2504,
        InvalidSource =2505,
        InvalidDestination = 2506,
        InvalidMessage = 2507,
        InactiveGateway= 2508,
        InternalServerError = 2509,
        InsufficientCredit = 2510
    }
}