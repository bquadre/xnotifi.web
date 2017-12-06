using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Controllers
{
    [Authorize]

    public class HomeController : Controller
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

        public HomeController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<GatewaysController> logger,
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
       
        public IActionResult Index()
        {
            ViewBagData();
            return View();
        }

        public async Task<IActionResult> Report(string s = "", string e = "")
        {

            try
            {
                var startDate = GetDateTime("2017-11-29").Date;
                var endDate = GetDateTime("2017-12-29").Date;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var requests = GetRequests().Where(x=>x.DateCreated.Date >= startDate && x.DateCreated.Date <= endDate);
                if (!isAdmin)
                {
                    requests = (List<RequestModel>)requests.Where(x => x.Application.Client.AspnetUserId.Equals(user.Id));
                }

              var  result = requests
                    .OrderByDescending(x => x.DateCreated)
                    .GroupBy(x => x.DateCreated.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Sms = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Recipients),
                        SmsCost = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Cost),
                        Email = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Recipients),
                        EmailCost = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Cost),
                    }).Distinct().ToList();

                ViewBag.RangeRequests = result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                //throw;
               var ex = exception.Message;
            }
            return View();
        }
        [HttpGet]
        [Route("[controller]/[action]")]
        public ChartModel Daily()
        {
            var last15Days = GetRequests()
                .OrderBy(x=>x.DateCreated)
                .GroupBy(x => x.DateCreated.Date)
                .Select(g => new {
                    Date = g.Key,
                    Sms = g.Where(x=>x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Recipients),
                    Email = g.Where(x=>x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Recipients),
                }).Distinct().TakeLast(15).ToList();

           
            var dates = new List<string>();
            var sms = new List<int>();
            var email = new List<int>();

            foreach (var item in last15Days)
            {
                dates.Add(item.Date.ToShortDateString());
                sms.Add(item.Sms);
                email.Add(item.Email);
            }

            var data = new ChartModel()
            {
                Dates = dates,
                Sms = sms,
                Email = email
            };
            return data;
        }


        [HttpGet]
        [Route("[controller]/[action]")]
        public ChartModel Monthly()
        {
            var monthly = GetRequests()
                .OrderBy(x => x.DateCreated)
                .GroupBy(x => x.DateCreated.Month)
                .Select(g => new {
                    Date = g.Key,
                    Sms = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Recipients),
                    Email = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Recipients),
                }).Distinct().TakeLast(6).ToList();


            var dates = new List<string>();
            var sms = new List<int>();
            var email = new List<int>();

            foreach (var item in monthly)
            {
                dates.Add(item.Date.ToString());
                sms.Add(item.Sms);
                email.Add(item.Email);
            }

            var data = new ChartModel()
            {
                Dates = dates,
                Sms = sms,
                Email = email
            };
            return data;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public PieChartModel Overall()
        {
            var overall = GetRequests()
                .OrderBy(x => x.DateCreated)
                .GroupBy(x => x.Gateway.ServiceCode)
                .Select(g => new {
                    Service = g.Key,
                    Sms = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Recipients),
                    Email = g.Where(x => x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Recipients),
                }).Distinct().ToList();


            var total = new List<int>
            {
                overall.Select(x => x.Sms).FirstOrDefault(),
                overall.Select(x => x.Email).FirstOrDefault()
            };

            var data = new PieChartModel()
            {
                Totals = total
            };
            return data;
        }

        private List<RequestModel> GetRequests()
        {
            return _requestService.List().Result;
        }

        private List<ApplicationModel> GetApplications()
        {
            return _applicationService.List().Result;
        }

        private DateTime GetDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Today.ToString();
            }

            return Convert.ToDateTime(date);
        }

        
        private void ViewBagData()
        {
            ViewBag.Default = 0;
            ViewBag.Requests = GetRequests().OrderByDescending(x=>x.DateCreated);
            ViewBag.AdminRequests = GetRequests().Count;
            //ViewBag.UserRequests = GetRequests().Where(x=>x.Application.Client.AspnetUserId.)

            ViewBag.AdminApplications = GetApplications().Count;
            ViewBag.AdminIncome= GetRequests().Sum(x=>x.Cost);
        }

    }
}
