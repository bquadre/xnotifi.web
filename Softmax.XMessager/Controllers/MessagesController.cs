using System;
using Microsoft.AspNetCore.Mvc;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Utitities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XMessager.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IOptions<PriceSettings> _priceSettings;
        private readonly IGenerator _generator;
        private readonly IMessageAdapter _messageAdapter;
        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;

        public MessagesController( 
            IOptions<PriceSettings> priceSettings,
            ILogger<MessagesController> logger,
            IGenerator generator,
            IMessageAdapter messageAdapter,

            IClientService clientService, 
            IGatewayService gatewayService,
            IApplicationService applicationService,
            IRequestService requestService
            )
        {
            _priceSettings = priceSettings;
            _generator = generator;

            _clientService = clientService;
            _gatewayService = gatewayService;
            _applicationService = applicationService;
            _requestService = requestService;
            _messageAdapter = messageAdapter;
        }
        // GET: /<controller>/
        [HttpPost]
        public MessageResponseModel Send(MessageRequestModel model)
        {

            if (!ModelState.IsValid)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InvalidFields,
                    Message = "Required parameter(s) are missing"
                };

            }
            var sms = model.Service.Equals(ServiceCode.Sms);
            var email = model.Service.Equals(ServiceCode.Email);

            if (!model.Service.Equals(ServiceCode.Sms) && !model.Service.Equals(ServiceCode.Email))
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InvalidService,
                    Message = "Invalid service request"
                };

            }

            var gateway = _gatewayService.List().Result
                .FirstOrDefault(x => x.ServiceCode == model.Service && x.IsActive == true);

            if (gateway == null)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InactiveGateway,
                    Message = "No active gateway available"
                };
            }

            var application = _applicationService.List().Result.FirstOrDefault(x =>
                x.ApplicationId.Equals(model.AppId) && x.Key.Equals(model.AppKey) && x.IsActive == true);
            if (application == null)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InvalidApplication,
                    Message = "No application record is found"
                };
            }

            var price = (model.Service == ServiceCode.Sms) ? _priceSettings.Value.Sms : _priceSettings.Value.Email;
            var destinations = model.Destination.Split(',');
            var cost = destinations.Length * price;

            var client = _clientService.Get(application.ClientId).Result;

            if (client == null)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InValidClient,
                    Message = "No client record is found"
                };
            }

            if (client.Balance < cost)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.InsufficientCredit,
                    Message = "Insufficient credit"
                };
            }

            gateway.Password = _generator.Decrypt(gateway.Password).Result;
            
            try
            {
                var gatewayResponse = "local testing";

                //var gatewayResponse = (model.Service == ServiceCode.Sms)
                //    ? _messageAdapter.SmsService(model, gateway)
                //    : _messageAdapter.Emailservice(model, gateway);


                client.Balance = client.Balance - cost;
                _clientService.Update(client);

                var request = new RequestModel()
                {
                    ApplicationId = application.ApplicationId,
                    GatewayId = gateway.GatewayId,
                    Recipients = destinations.Length,
                    Cost = cost,
                    Response = gatewayResponse,
                    DateCreated = DateTime.UtcNow
                };

               var lastRequest = _requestService.Create(request).Result;

                return new MessageResponseModel()
                {
                    Error = Data.Enums.StatusCode.MessageSubmitted,
                    Message = "Message(s) submited successfully",
                    Reference = lastRequest.RequestId
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }



            var response = new MessageResponseModel()
            { 
                Error = Data.Enums.StatusCode.InternalServerError,
                Message ="Internal server error"
            };
            return response;
        }
      
    }
}

