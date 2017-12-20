using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models
{
    public class ApplicationModel
    {
        public string ApplicationId { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ClientModel Client { get; set; }
    }
}
