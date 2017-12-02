using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Softmax.XMessager.Data.Contracts.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XMessager.Controllers
{
    [Authorize(Roles="Admin, Manager")]
    public class RequestsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IGenerator _generator;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;

        public RequestsController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<RequestsController> logger,
            IGenerator generator,

            IClientService clientService, 
            IGatewayService gatewayService,
            IApplicationService applicationService,
            IRequestService requestService
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
            _requestService = requestService;
        }
        // GET: /<controller>/
        public IActionResult Index(string s, string q, int page=1)
        {
            SelectList();
            var request = _requestService.List(s, q);
            var response = (request.Successful) ? request.Result : null;
            return View(response);
        }

        private void SelectList()
        {
            ViewBag.Applications =
                new SelectList(GetApplications(), "ApplicationId", "Name", "--Select One--");
        }
        private List<ApplicationModel> GetApplications()
        {
            return _applicationService.List().Result;
        }
    }
}

