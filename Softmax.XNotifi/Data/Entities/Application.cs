using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XNotifi.Data.Entities
{
    public class Application
    {
        [Key]
        public string ApplicationId { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Client Client { get; set; }
    }
}