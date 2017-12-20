using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, Display(Name = "Enter your username")]     
        public string Email { get; set; }
    }
}
