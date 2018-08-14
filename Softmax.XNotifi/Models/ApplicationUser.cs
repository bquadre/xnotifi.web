using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Softmax.XNotifi.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string UnqueNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Organisation { get; set; }

        public string Address { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Balance { get; set; } = 0.00m;

        public string Code { get; set; }

        public DateTime? CodeExpired { get; set; }

        public bool IsActive { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsTempPassword { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
