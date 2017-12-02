using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Controllers
{
    public class BaseController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public BaseController()
        {
                
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}