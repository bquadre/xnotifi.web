using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Softmax.XNotifi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Softmax.XNotifi.Data.Contracts.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XNotifi.Controllers
{

    public class ApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private readonly IClientService _clientService;
        private readonly IApplicationService _applicationService;

        public ApplicationsController(UserManager<ApplicationUser> userManager,
            ILogger<GatewaysController> logger,

            IClientService clientService, 
            IGatewayService gatewayService,
            IApplicationService applicationService
            )
        {
            _userManager = userManager;
            _logger = logger;
            _clientService = clientService;
            _applicationService = applicationService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBagData();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBagData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationModel model)
        {
            ViewBagData();
            if (!ModelState.IsValid) return View(model);
            try
            {
                if (model.ClientId == null)
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                    var client = _clientService.List().FirstOrDefault(x => x.AspnetUserId.Equals(user.Id));
                    model.ClientId = client.ClientId;
                }

                var check = _applicationService.List().FirstOrDefault(x =>
                    x.ClientId == model.ClientId && x.Name.ToLower().Equals(model.Name.ToLower()));
                if (check != null)
                {
                    TempData["error"] = model.Name +" already exist";
                    return RedirectToAction("Index", "Home");

                }
                _applicationService.Create(model);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction("Index", "Applications");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBagData();
            var result = _applicationService.Get(id);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationModel model)
        {
            ViewBagData();
            if (!ModelState.IsValid) return View(model);
            try
            {
                var existingApplication = _applicationService.Get(model.ApplicationId);
                var check = _applicationService.List().FirstOrDefault(x =>
                    x.ClientId == model.ClientId  && x.Name.ToLower().Equals(model.Name.ToLower()) && 
                    !x.Name.ToLower().Equals(existingApplication.Name.ToLower()));
                if (check != null)
                {
                    TempData["error"] = model.Name + " already exist";
                    return RedirectToAction("Index", "Home");

                }
                _applicationService.Update(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var err = ex.Message;
            }
            return View(model);
        }

        private async Task<List<ApplicationModel>> GetApplications()
        {
            var currentUser = GetCurrentClient().GetAwaiter().GetResult();
            var result = _applicationService.List();
            var admin = await IsAdmin();
            return admin ? result.ToList() : result.Where(x => x.ClientId.Equals(currentUser.ClientId)).ToList();
        }

        private async Task<ClientModel> GetCurrentClient()
        {
            var identity = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(identity);

            var list = _clientService.List();
            var client = list.FirstOrDefault(x => x.AspnetUserId.Equals(user.Id));
            return client;
        }

        private async Task<bool> IsAdmin()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.IsInRoleAsync(user, "Admin");
            return role;
        }

        private void ViewBagData()
        {
            var applications = GetApplications().GetAwaiter().GetResult().ToList();
            ViewBag.Clients = new SelectList(GetClients(), "ClientId", "Company", "--Select One--");
            ViewBag.Applications = applications;
        }

        private List<ClientModel> GetClients()
        {
            return _clientService.List().ToList();
        }
    }
}

