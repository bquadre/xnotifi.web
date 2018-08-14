using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Rest.Azure;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Models;
using Softmax.XNotifi.Models.AccountViewModels;

namespace Softmax.XNotifi.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IRequestService _requestService;
        private readonly IMessageFactory _messageFactory;
        private readonly IGenerator _generator;
      

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,

            IClientService clientService,
            IGatewayService gatewayService,
            IRequestService requestService,
            IGenerator generator,
            IMapper mapper,
            IMessageAdapter messageAdapter,
            IMessageFactory messageFactory,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            
            _clientService = clientService;
            _generator = generator;
            _messageFactory = messageFactory;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBagData();
            return View();
        }
       
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ClientModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser { UserName = model.EmailAddress, Email = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                model.AspnetUserId = user.Id;
               var client = _clientService.Create(model).Result;
                //send confirmation email
                _messageFactory.SendEmailConfirmationLink(client, user.Id);
                _logger.LogInformation("User created a new account with password.");
                TempData["success"] = "Registration successful. Email confirmation link has been sent to " +
                                      model.EmailAddress;
                return PartialView("_Message");
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
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
 
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return View(model);
            var client = _clientService.List().FirstOrDefault(x => x.AspnetUserId == user.Id);
            client.Code = _generator.RandomNumber(1000, 9999).Result;
            client.CodeExpired = DateTime.UtcNow.AddHours(1);
            var updateClient = _clientService.Update(client).Result;
            _messageFactory.SendEmailForgotPasswordLink(updateClient);

            /*
            var tempPassword = _generator.TempPassword(7);
            var passwordHasher = _userManager.PasswordHasher.HashPassword(user, tempPassword);
            user.PasswordHash = passwordHasher;
            user.IsTempPassword = true;
            await _userManager.UpdateAsync(user);

            var subject = "Forgot Password";
            var message = "<br/><br/>";
            message += "Dear " + name + ",";
            message += "<br/><br/>";
            message += "You recently requested for password reset on SBC website below is a temporary password";
            message += "<br/><br/>";
            message += "<b>Unique ID:</b> " + uniqueId;
            message += "<br/>";
            message += "<b>Temporary Password:</b> " + tempPassword;
            message += "<br/><br/>";
            message += "Thank You.";
            message += "<br/><br/>";
            message += "Regards,<br/>";
            message += "SBC Team";

            var filename = Path.Combine(
                  Directory.GetCurrentDirectory(), "wwwroot/templates",
                  "NotificationContent.html");
            string content = System.IO.File.ReadAllText(filename);
            content = content.Replace("#NotificationHeader", subject);
            content = content.Replace("#NotificationBody", message);
            content = content.Replace("#YEAR", DateTime.Now.Year.ToString());

            Notifi.SendEmail(subject, content, model.Email, "NotificationLayout.html");

            ViewBag.Success = "A temporary password has been sent to " + model.Email;
            return View(model);
            */

            TempData["success"] = "Please check your email for  password reset link";
            return PartialView("_Message");
            // If we got this far, something failed, redisplay form
        }

        [HttpGet]
        public IActionResult ChangePassword(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var identityUser = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(identityUser);
            var check = await _userManager.CheckPasswordAsync(user, model.Password);
            if (check)
            {

                try
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, model.ConfirmPassword);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            //AddErrors(user);

            // If we got this far, something failed, redisplay form
            return View(model);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string id = null)
        {
            if (id == null)
            {
                TempData["error"] = "Required parameter is missing";
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordViewModel(){Code = id};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientService.List().FirstOrDefault(x =>
                x.ClientId == model.Code);
            if (client == null)
            {
                TempData["error"] = "Validation code is invalid";
                return PartialView("_Message");
            }

            if (client.CodeExpired < DateTime.UtcNow)
            {
                TempData["error"] = "Validation code has expired";
                return PartialView("_Message");
            }

            var user = await _userManager.FindByIdAsync(client.AspnetUserId);
            if (user == null) return View(model);
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, model.ConfirmPassword);
                TempData["success"] = "Account password reset successful";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            var result = _clientService.Get(id);
            return View(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ClientModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _clientService.Update(model);
                TempData["success"] = "Profile has been updated";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                var err = ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
                Name = GetClientContact(user.Id),
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
        public async Task<IActionResult> Profile()
        {
            ViewBag.EmailConfirmed = false;
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user.EmailConfirmed)
            {
                ViewBag.EmailConfirmed = true;
            }

            var client = await GetCurrentClient();
            var request = _clientService.Get(client.ClientId);
            return View(request);
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
        public async Task<IActionResult> Confirm(string id=null)
        {
            var result = await _userManager.FindByIdAsync(id);

            if (result == null)
            {
                TempData["error"] = "No user record is found";
            }

            else if (result.EmailConfirmed)
            {
                TempData["error"] = result.Email + " has already been confirmed";

            }
            else
            {
                result.EmailConfirmed = true;
                await _userManager.UpdateAsync(result);
                TempData["success"] = result.Email + " has been confirmed";

                return RedirectToAction("Login");
            }
            return PartialView("_Message");
        }

        public async Task<IActionResult> AccessKey()
        {
            var client = await GetCurrentClient();
            var model = _clientService.Get(client.ClientId);
            var key = _generator.GenerateGuid().Result;

            model.AccessKey = key;
            _clientService.Update(model);

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> ResendLink()
        {
            var client = await GetCurrentClient();
            _messageFactory.SendEmailConfirmationLink(client, client.AspnetUserId);
            TempData["success"] = "Email confirmation link has been sent";
            return RedirectToAction("Index", "Home");
        }

        private string GetClientContact(string userId)
        {
            var client = _clientService.List().FirstOrDefault(x => x.AspnetUserId == userId);
            if (client == null)
            {
                return null;
            }

            return client.LastName + " " + client.FirstName;
        }

        private void ViewBagData()
        {
            ViewBag.Clients = _clientService.List()
                .Where(x=>x.IsDeleted == false)
                .ToList();
        }
        
        #region Helpers
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

        private async Task<ClientModel> GetCurrentClient()
        {
            var identity = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(identity);

            var list = _clientService.List();
            var client = list.FirstOrDefault(x => x.AspnetUserId.Equals(user.Id));
            return client;

        }
        #endregion
    }
}