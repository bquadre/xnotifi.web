using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Utitities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XNotifi.Controllers
{
    [Authorize(Roles="Admin, Manager")]
    public class PaymentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IOptions<XNotifiSettings> _xnotifiSettings;
        private readonly ILogger _logger;
        private readonly IGenerator _generator;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<XNotifiSettings> xnotifiSettings,
            ILogger<PaymentsController> logger,
            IGenerator generator,

            IClientService clientService, 
            IGatewayService gatewayService,
            IPaymentService paymentService)
            {
                _userManager = userManager;
                _logger = logger;
                _generator = generator;

                _clientService = clientService;
                _gatewayService = gatewayService;
                _paymentService = paymentService;
                _xnotifiSettings = xnotifiSettings;
            }
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBagData();
            return View();
        }

        public IActionResult Create(string Id)
        {
            ViewBagData();
            var model = new PaymentModel() {ClientId = Id};
            return View(model);
        }

        public IActionResult Edit(string id)
        {
           SelectList();

            var request = _gatewayService.Get(id);
            //var response = (request.Successful) ? request. : null;
            request.Password = _generator.Decrypt(request.Password).Result;
            return View(request);
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GatewayModel model)
        {
            SelectList();
            if (!ModelState.IsValid) return View(model);
            try
            {
                

                _gatewayService.Create(model);

                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction("Index", "Gateways");
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
        public IActionResult Edit(GatewayModel model)
        {
            SelectList();
            if (!ModelState.IsValid) return View(model);
            try
            {
                _gatewayService.Update(model);
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
            ViewBag.Services =
                new SelectList(GetServices(), "Key", "Value", "--Select One--");
        }
        private Dictionary<int, string> GetServices()
        {
            var services = new Dictionary<int, string>();
            services.Add((int)ServiceCode.Sms, "SMS");
            services.Add((int)ServiceCode.Email, "Email");

            return services;
        }

        private Dictionary<string, string> GetPaymentMethods()
        {
            var methods = _xnotifiSettings.Value.PaymentMethods.Split(',');

            return methods.ToDictionary(item => item);
        }

        private Dictionary<int, string> GetPaymentStatus()
        {
            var status = new Dictionary<int, string>();
            status.Add((int)PaymentStatus.Pending, PaymentStatus.Pending.ToString());
            status.Add((int)PaymentStatus.Authorized, PaymentStatus.Authorized.ToString());
            status.Add((int)PaymentStatus.Declined, PaymentStatus.Declined.ToString());
            status.Add((int)PaymentStatus.Cancelled, PaymentStatus.Cancelled.ToString());
            status.Add((int)PaymentStatus.Refunded, PaymentStatus.Refunded.ToString());
            return status;
        }

        private List<ClientModel> GetClients()
        {
            return _clientService.List().ToList();
        }

        private async Task<List<PaymentModel>> GetPayments()
        {
            var currentUser = GetCurrentClient().GetAwaiter().GetResult();
            var result = _paymentService.List();
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
            var payments = GetPayments().GetAwaiter().GetResult();
            ViewBag.Clients = new SelectList(GetClients(), "ClientId", "Company", "--Select One--");
            ViewBag.PaymentMethods = new SelectList(GetPaymentMethods(), "Key", "Value", "--Select One--");
            ViewBag.PaymentStatus = new SelectList(GetPaymentStatus(), "Key", "Value", "--Select One--");
            ViewBag.Payments = payments.ToList();
        }
    }
}

