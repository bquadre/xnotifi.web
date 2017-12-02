using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softmax.XMessager.Data.Enums
{
    public enum ErrorCode
    {
        MessageSubmitted =2500,
        InValidFields =2501,
        InValidService = 2502,
        InvalidApplication =2503,
        InValidClient =2504,
        InvalidSource =2505,
        InvalidDetination = 2506,
        InvalidMessage = 2507,
        ResponseTimeOut= 2508,
        InternalServerError = 2509,
        InsufficientCredit = 2510
    }
}