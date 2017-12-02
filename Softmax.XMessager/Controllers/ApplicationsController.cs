using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Softmax.XMessager.Data.Contracts.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XMessager.Controllers
{
    [Authorize(Roles="Admin, Manager")]
    public class ApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IGenerator _generator;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IApplicationService _applicationService;

        public ApplicationsController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<GatewaysController> logger,
            IGenerator generator,

            IClientService clientService, 
            IGatewayService gatewayService,
            IApplicationService applicationService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
            _generator = generator;

            _clientService = clientService;
            _gatewayService = gatewayService;
            _applicationService = applicationService;
        }
        // GET: /<controller>/
        public IActionResult Index(string q,int page=1)
        {
            var request = _applicationService.List(q);
            var response = (request.Successful) ? request.Result : null;
            return View(response);
        }

        public IActionResult Create()
        {
            SelectList();
            return View();
        }

        public IActionResult Edit(string id)
        {
           SelectList();

            var request = _applicationService.Get(id);
            var response = (request.Successful) ? request.Result : null;
            return View(response);
        }

        public IActionResult NewKey(string id)
        {
            var request = _applicationService.Get(id).Result;
            request.Key = _generator.RandomNumber(1000000, 9999999).Result;
            _applicationService.Update(request);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationModel model)
        {
            SelectList();
            if (!ModelState.IsValid) return View(model);
            try
            {
                _applicationService.Create(model);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction("Index", "Applications");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
                //AddErrors(ex);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationModel model)
        {
            SelectList();
            if (!ModelState.IsValid) return View(model);
            try
            {
                _applicationService.Update(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                var err = ex.Message;
            }

            return View(model);
        }



        private void SelectList()
        {
            ViewBag.Clients =
                new SelectList(GetClients(), "ClientId", "Company", "--Select One--");
        }

        private List<ClientModel> GetClients()
        {
            return _clientService.List().Result;
        }
        private Dictionary<int, string> GetServices()
        {
            var services = new Dictionary<int, string>();
            services.Add((int)ServiceCode.Sms, "SMS");
            services.Add((int)ServiceCode.Email, "Email");

            return services;
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
    
    }
}

