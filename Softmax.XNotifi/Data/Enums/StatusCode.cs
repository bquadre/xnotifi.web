using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softmax.XNotifi.Data.Enums
{
    public enum StatusCode
    {
        SENT =2500,
        INCOMPLETEFIELDS =2501,
        INVALIDACCOUNT = 2502,
        GATEWAYERROR= 2503,
        INTERNALSERVERERROR = 2504,
        INSUFFICIENTCREDIT = 2505
    }
}