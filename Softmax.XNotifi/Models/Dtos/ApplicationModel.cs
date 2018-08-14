using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XNotifi.Models
{
    public class ApplicationModel
    {
        public string ApplicationId { get; set; }
        [Display(Name = "Client")]
        public string ClientId { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ClientModel Client { get; set; }
    }
}
