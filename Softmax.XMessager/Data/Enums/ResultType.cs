using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XMessager.Data.Enums
{
    public enum ResultType
    {
        ValidationError,
        Error,
        Success,
        Warning,
        Empty,
        InsufficientBalance,
        PendingTransaction,
        DataIntegrity
    }
}
