using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models
{
    public class RequestRangeModel
    {
        public string Date { get; set; }
        public int Sms { get; set; }
        public decimal SmsCost { get; set; }
        public int Email { get; set; }
        public decimal EmailCost { get; set; }
    }
}
