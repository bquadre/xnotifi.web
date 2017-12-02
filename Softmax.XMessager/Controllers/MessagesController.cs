using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Utitities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XMessager.Controllers
{
    public class MessagesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<PriceSettings> _priceSettings;

        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IGenerator _generator;

        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;

        public MessagesController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<PriceSettings> priceSettings,
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
            _priceSettings = priceSettings;
            _emailSender = emailSender;
            _logger = logger;
            _generator = generator;

            _clientService = clientService;
            _gatewayService = gatewayService;
            _applicationService = applicationService;
            _requestService = requestService;
        }
        // GET: /<controller>/
        [HttpPost]
        public MessageResponseModel Send(MessageRequestModel model)
        {
            //get pricing from web cofig
            //check app exist
            //check credit bal
            //check service

            if (!ModelState.IsValid)
            {
                return new MessageResponseModel()
                {
                    Error = true,
                    Message = "Required parameter(s) are missing"
                };

            }

            var application = _applicationService.List().Result.FirstOrDefault(x =>
                x.ApplicationId.Equals(model.AppId) && x.Key.Equals(model.AppKey) && x.IsActive == true);
            if (application == null)
            {
                return new MessageResponseModel()
                {
                    Error = true,
                    Message = "No application record is found"
                };
            }

            var price = (model.Service == ServiceCode.Sms) ? _priceSettings.Value.Sms : _priceSettings.Value.Email;
            var receivers = model.Recipients.Split(',');
            var cost = receivers.Length * price;

            var client = _clientService.Get(application.ClientId).Result;

            if (client == null)
            {
                return new MessageResponseModel()
                {
                    Error = true,
                    Message = "No client record is found"
                };
            }

            if (client.Balance < cost)
            {
                return new MessageResponseModel()
                {
                    Error = true,
                    Message = "Insufficient fund"
                };
            }

            var gateway = _gatewayService.List().Result
                .FirstOrDefault(x => x.ServiceCode == model.Service && x.IsActive == true);

            if (gateway == null)
            {
                return new MessageResponseModel()
                {
                    Error = true,
                    Message = "Could not connect to the gatway at this time"
                };
            }

            try
            {
               // if()
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



            var response = new MessageResponseModel()
            { 
                Error = true,
                Message ="Error"
            };

            return response;
        }

        
        private List<ApplicationModel> GetApplications()
        {
            return _applicationService.List().Result;
        }
    }
}

