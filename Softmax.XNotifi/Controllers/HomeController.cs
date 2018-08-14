using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;
        private readonly IClientService _clientService;

        public HomeController(UserManager<ApplicationUser> userManager,
            IClientService clientService,
            IApplicationService applicationService,
            IRequestService requestService)
            {
                _userManager = userManager;
                _clientService = clientService;
                _applicationService = applicationService;
                _requestService = requestService;
            }
       
        public IActionResult Index()
        {
            ViewBagData();
            return View();
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ChartModel> Daily()
        {
            var result = await GetRequests();
            var last15Days = result
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
        public async Task<ChartModel> Monthly()
        {
            var result = await GetRequests();
            var monthly = result
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
        public async Task<PieChartModel> Overall()
        {
            var result = await GetRequests();
            var sms = result.Where(x=>x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x=>x.Recipients);
            var email = result.Where(x => x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Recipients);
            
            var data = new PieChartModel()
            {
                Totals = new List<int> { sms, email }
            };

            return data;
        }

        private async Task<List<ApplicationModel>> GetApplications()
        {
            var currentUser = GetCurrentClient().GetAwaiter().GetResult();
            var result = _applicationService.List();
            var admin = await IsAdmin();
            return admin ? result.ToList() : result.Where(x => x.ClientId.Equals(currentUser.ClientId)).ToList();
        }

        private async Task<List<RequestModel>> GetRequests()
        {
            var currentUser = GetCurrentClient().GetAwaiter().GetResult();
            var result = _requestService.List();
            var admin = await IsAdmin();
            return admin ? result.ToList() : result.Where(x => x.Application.ClientId.Equals(currentUser.ClientId)).ToList();
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
            var requests = GetRequests().GetAwaiter().GetResult().ToList();
            var applications = GetApplications().GetAwaiter().GetResult().ToList();
            var currentUser = GetCurrentClient().GetAwaiter().GetResult();
            var isAdmin = IsAdmin().GetAwaiter().GetResult();
            var balance = currentUser.Balance;
            var income = requests.Sum(x => x.Cost);

            ViewBag.Default = 0;

            ViewBag.Requests = requests.OrderByDescending(x => x.DateCreated);
            ViewBag.RequestsCount = requests.Count;
            ViewBag.ApplicationsCount = applications.Count;

            ViewBag.BalanceIncome = (isAdmin) ? income : balance;
            ViewBag.BalanceIncomeText = (isAdmin) ? "Total Income" : "Account Balance";

            ViewBag.SMSCost = requests.Where(x=>x.Gateway.ServiceCode == ServiceCode.Sms).Sum(x => x.Cost);
            ViewBag.EmailCost = requests.Where(x=>x.Gateway.ServiceCode == ServiceCode.Email).Sum(x => x.Cost);
            ViewBag.TotalCost = ViewBag.SmsCost + ViewBag.EmailCost;
        }

    }
}
