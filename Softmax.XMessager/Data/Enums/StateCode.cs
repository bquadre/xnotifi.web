using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Softmax.XMessager.Data.Enums
{
    public enum StateCode
    {
        [Description("None")]
        None = 1,

        [Description("Lagos")]
        Lagos = 2,

        [Description("Ibadan")]
        Ibadan = 3,

        [Description("Ondo")]
        Ondo = 4, 
    }
}