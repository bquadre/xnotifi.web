using Softmax.XNotifi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XNotifi.Data.Entities
{
    public class Client
    {
        [Key]
        public string ClientId { get; set; }
        public string AspnetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string AccessKey { get; set; }
        public string Company { get; set; }
        public decimal Balance { get; set; } = 0.00m;
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }
        public string Code { get; set; }
        public DateTime? CodeExpired { get; set; }
        public bool IsActive { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ApplicationUser AspNetUser { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}