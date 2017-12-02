using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Models;
using Softmax.XMessager.Models.AccountViewModels;

namespace Softmax.XMessager.Controllers
{
    [Authorize]
    //[Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IRequestService _requestService;
        private readonly IGenerator _generatorService;
        private readonly IMapper _mapper;
      

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,

            IClientService clientService,
            IGatewayService gatewayService,
            IRequestService requestService,
            IGenerator generatorService,
            IMapper mapper,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            
            _clientService = clientService;
            _generatorService = generatorService;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult Index(string q, int page = 1)
        {
            var request = _clientService.List(q);
            var response = (request.Successful) ? request.Result : null;
            return View(response);
        }
        
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult Edit(string id)
        {
            var request = _clientService.Get(id);
            var response = (request.Successful) ? request.Result : null;
            return View(response);
        }

        public async Task<IActionResult> Roles(string id)
        {
            if (id == null)
                return BadRequest("id can not be null");
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest("Not Found");

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new UserRoleViewModel
            {
                Id = user.Id,
                //Name = GetEmployeeName(user.Id),
                RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ClientModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser { UserName = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, "Password@1");
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                model.AspnetUserId = user.Id;
                _clientService.Create(model);
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                // await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                // await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction("Index", "Account");
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ClientModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _clientService.Update(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                var err = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Roles(UserRoleViewModel editUserRole, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUserRole.Id);
                if (user == null)
                    return BadRequest("no record found");


                var userRoles = await _userManager.GetRolesAsync(user);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index", "Account");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }







        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction(nameof(HomeController.Index), "Home");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
                throw new ApplicationException("A code must be supplied for password reset.");
            var model = new ResetPasswordViewModel {Code = code};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //custom methods

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Employee()
        //{
        //    ViewBag.genders =
        //        new SelectList(GetGenders(), "Key", "Value", "--Select One--");
          
        //}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Customer()
        {
            ViewBag.genders =
                new SelectList(GetGenders(), "Key", "Value", "--Select One--");
            return View();
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Savings()
        //{
        //    DepositSelectLists();
        //    return View();
        //}

         
       

   
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult Savings(DepositModel model)
        //{
        //    DepositSelectLists();

        //    if (ModelState.IsValid)
        //    {
        //        model.EmployeeId = CurrentEmployee().EmployeeId;
        //        model.TransactionCode = model.TransactionCode == (GatewayType) 1
        //            ? GatewayType.Deposit
        //            : GatewayType.Withdrawal;

        //        var request = depositService.Create(model);
        //        if (request.Successful)
        //            return RedirectToAction("Index", "Savings");

        //        if (request.ResultType == ResultType.InsufficientBalance)
        //            TempData["Error"] = "Insufficient Balance";

        //        if (request.ResultType == ResultType.PendingTransaction)
        //            TempData["Error"] = "This custormer has a pending transaction";
        //    }


        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        
        #region Helpers

        private async Task<string> GenerateEmployeeLoginId()
        {
            var id = string.Empty;

            for (var i = 0; i < 1000; i++)
            {
                id = _generatorService
                    .RandomNumber(1000, 1999).Result;
                var check = await _userManager.FindByNameAsync(id);
                if (check == null)
                    break;
            }

            return id;
        }

        private async Task<string> GenerateCustomerLoginId()
        {
            var id = string.Empty;

            for (var i = 0; i < 1000; i++)
            {
                id = _generatorService
                    .RandomNumber(2000, 9999).Result;
                var check = await _userManager.FindByNameAsync(id);
                if (check == null)
                    break;
            }

            return id;
        }

        private string GenerateTempPassword()
        {
            var password = _generatorService
                .GenerateGuid()
                .Result.Substring(0, 8);
            return password;
        }

        private Dictionary<int, string> GetGenders()
        {
            var genders = new Dictionary<int, string>();
            genders.Add((int) GenderCode.Female, "Female");
            genders.Add((int) GenderCode.Male, "Male");

            return genders;
        }

        

        //private List<GatewayModel> GetBranches()
        //{
        //    var result = branchService.List().Result.ToList();
        //    return result;
        //}

        //private List<UserModel> GetSavingsProducts()
        //{
        //    var result = _productService.List("").Result
        //        .Where(x => x.ProductCode == ServiceCode.Savings && x.IsDeleted == false).ToList();
        //    return result;
        //}

        //private List<UserModel> GetLoansProducts()
        //{
        //    var result = _productService.List().Result
        //        .Where(x => x.ProductCode == ServiceCode.Loans && x.IsDeleted == false).ToList();
        //    return result;
        //}

        //private List<RequestModel> GetCustomers()
        //{
        //    var result = customerService.List().Result.ToList();
        //    return result;
        //}

        //private void DepositSelectLists()
        //{
        //    ViewBag.products =
        //        new SelectList(GetSavingsProducts(), "ProductId", "Name", "--Select One--");

        //    ViewBag.customers =
        //        new SelectList(GetCustomers(), "CustomerId", "AccountNumber", "--Select One--");

        //    ViewBag.TransactionCodes =
        //        new SelectList(GetTransactionCodes(), "Key", "Value", "--Select One--");
        //}

        //private void LoanSelectLists()
        //{
        //    ViewBag.Products =
        //        new SelectList(GetLoansProducts(), "ProductId", "Name", "--Select One--");

        //    ViewBag.Customers =
        //        new SelectList(GetCustomers(), "CustomerId", "AccountNumber", "--Select One--");
        //}

        //private EmployeeModel CurrentEmployee()
        //{
        //    var user = User.Identity.Name;
        //    var currentUser = employeeService.List().Result
        //        .FirstOrDefault(x => x.EmployeeNumber.Contains(user));

        //    return currentUser;
        //}

        //private string GetBranchCode()
        //{
        //    var branchId = CurrentEmployee().BranchId;
        //    var code = branchService.Get(branchId).Result;
        //    return code.BranchCode;
        //}

        //private Dictionary<int, string> GetTransactionCodes()
        //{
        //    var transactionCodes = new Dictionary<int, string>();
        //    transactionCodes.Add((int) GatewayType.Deposit, "Deposit");
        //    transactionCodes.Add((int) GatewayType.Withdrawal, "Withdrawal");

        //    return transactionCodes;
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //private void RefundViewBagData(string id)
        //{
        //    var result = loanService.List().Result.FirstOrDefault(x => x.LoanId == id);
        //    if (result != null)
        //    {
        //        ViewBag.LoanId = id;
        //        ViewBag.CustomerId = result.CustomerId;
        //        ViewBag.Customer = result.Customer.FirstName + " " + result.Customer.LastName;
        //        ViewBag.AccountNumber = result.Customer.AccountNumber;
        //    }
        //}

        #endregion
    }
}