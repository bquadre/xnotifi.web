using System.ComponentModel;

namespace Softmax.XMessager.Data.Enums
{
    public enum ServiceCode
    {
        [Description("None")]
        None = 0,

        [Description("SMS")]
        Sms = 1,

        [Description("Email")]
        Email = 2
    }
}