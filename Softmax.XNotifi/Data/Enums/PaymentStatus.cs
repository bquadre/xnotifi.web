using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;

namespace Softmax.XNotifi.Data.Enums
{
    public enum PaymentStatus
    {
        PENDING = 0,
        CONFIRMED = 1,
        DECLINED = 2,
        CANCELLED = 3,
        REFUNDED = 4
    }
}
