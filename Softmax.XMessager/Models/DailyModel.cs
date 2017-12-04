using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XMessager.Models
{
    public class DailyModel
    {
        public List<string> Dates { get; set; }
        public List<int> Sms { get; set; }
        public List<int> Email { get; set; }
    }
}
