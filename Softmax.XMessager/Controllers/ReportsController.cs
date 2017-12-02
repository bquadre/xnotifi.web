using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Models;
using Softmax.XMessager.Models.Reports;

namespace Softmax.XMessager.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ReportsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IGatewayService _branchService;
        private readonly IRequestService _customerService;
        private readonly IDepositService _depositService;
        private readonly IDepositConfirmService _depositconfirmService;
        private readonly IEmployeeService _employeeService;
        private readonly ILoanService _loanService;
        private readonly ILoanApprovalService _loanapprovalService;
        private readonly IUserService _productService;
        private readonly IRefundService _refundService;
        private readonly IRefundConfirmService _refundconfirmService;

        public ReportsController(

            IGatewayService branchService,
            IUserService productService,
            IRequestService customerService,
            IEmployeeService employeeService,
            IDepositService depositService,
            IDepositConfirmService depositconfirmService,
            ILoanService loanService,
            ILoanApprovalService loanapprovalService,
            IRefundService refundService,
            IRefundConfirmService refundconfirmService)
        {
            _branchService = branchService;
            _productService = productService;
            _customerService = customerService;
            _employeeService = employeeService;
            _depositService = depositService;
            _depositconfirmService = depositconfirmService;
            _loanService = loanService;
            _loanapprovalService = loanapprovalService;
            _refundService = refundService;
            _refundconfirmService = refundconfirmService;

        }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> Summary(string s = "", string e="")
        {

            //var startDate = GetDateTime(s).Date;
            //var endDate = GetDateTime(e).Date;

            var startDate = GetDateTime("2017-11-29").Date;
            var endDate = GetDateTime("2017-11-29").Date;

            var deposits = GetDeposits(startDate, endDate);

            /*
             * 
             * List<Product> Lines = LoadProducts();    
List<ResultLine> result = Lines
                .GroupBy(l => l.ProductCode)
                .SelectMany(cl => cl.Select(
                    csLine => new ResultLine
                    {
                        ProductName =csLine.Name,
                        Quantity = cl.Count().ToString(),
                        Price = cl.Sum(c => c.Price).ToString(),
                    })).ToList<ResultLine>();
             * */

           var depositsResult = new List<DepositResult>();
            if (deposits != null)
            {
                depositsResult = deposits
                   .GroupBy(emp => emp.EmployeeId)
                   .SelectMany(ea => ea.Select(
                       empAmount => new DepositResult
                       {
                           Name = empAmount.Employee.FirstName,
                           Amount = ea.Sum(x => x.Amount).ToString()
                       })).Distinct().ToList<DepositResult>();

                var query = from d in deposits
                    group d by d.EmployeeId into grp
                    select new
                    {
                        Id = grp.Key,
                        Name = grp.FirstOrDefault(x => x.Customer.CustomerId == grp.Key),
                        Sum = grp.Sum(r => r.Amount)
                    };
            }

            var result = depositsResult;
            return View();
        }

        private DateTime GetDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Today.ToString();
            }

            return Convert.ToDateTime(date);
        }

        private List<DepositModel> GetDeposits(DateTime startDate, DateTime endDate)
        {
            return _depositService.List().Result.Where(
                x =>x.TransactionCode == GatewayType.Deposit &&
                x.StatusCode == Data.Enums.StatusCode.Confirmed && 
                x.DateCreated.Date >= startDate &&
                x.DateCreated.Date <= endDate).ToList();
        }

    }
}