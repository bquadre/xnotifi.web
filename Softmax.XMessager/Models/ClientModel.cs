using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
//using Softmax.XMessager.Data.Entities;

namespace Softmax.XMessager.Models
{
    public class ClientModel
    {
        public string ClientId { get; set; }
        public string AspnetUserId { get; set; }
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, Display(Name = "Organisation")]
        public string Company { get; set; }
        public string Role { get; set; }
        public decimal Balance { get; set; } = 0.00m;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        [Required, Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public virtual ApplicationUser AspNetUser { get; set; }
        public virtual ICollection<ApplicationModel> Applications { get; set; }
    }
}
