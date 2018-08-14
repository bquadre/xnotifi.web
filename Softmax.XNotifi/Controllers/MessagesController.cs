using System;
using Microsoft.AspNetCore.Mvc;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Utitities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XNotifi.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IOptions<XNotifiSettings> _priceSettings;
        private readonly IGenerator _generator;
        private readonly IMessageAdapter _messageAdapter;
        private readonly IClientService _clientService;
        private readonly IGatewayService _gatewayService;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;

        public MessagesController( 
            IOptions<XNotifiSettings> priceSettings,
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
        [HttpGet]
        public MessageResponseModel Send(string clientId, string accessKey, ServiceCode service, string from, string to, string message, string subject="", string brand="")
        {
            var model = new MessageRequestModel()
            {
                ClientId = clientId,
                AccesssKey = accessKey,
                Service = service,
                From =  from,
                To = to,
                Message = message,
                Subject = subject,
                Brand =  brand
            };

           return Messager(model);
        }

        // Post: /<controller>/
        [HttpPost]
        public MessageResponseModel Send(MessageRequestModel model)
        {
            return Messager(model);
        }
       
        private MessageResponseModel Messager(MessageRequestModel model)
        {

            if(!ModelState.IsValid)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.InvalidFields,
                    Message = "Required parameter(s) are missing"
                };

            }

            if (!model.Service.Equals(ServiceCode.Sms) && !model.Service.Equals(ServiceCode.Email))
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.InvalidService,
                    Message = "Invalid service request"
                };

            }

            var gateway = _gatewayService.List()
                .FirstOrDefault(x => x.ServiceCode == model.Service && x.IsActive == true);

            if (gateway == null)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.InactiveGateway,
                    Message = "No active gateway available"
                };
            }

            var client = _clientService.List().ToList().FirstOrDefault(x =>
                x.ClientId.Equals(model.ClientId) && x.AccessKey.Equals(model.AccesssKey) && x.IsActive == true);
            if (client == null)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.InvalidApplication,
                    Message = "No client record is found"
                };
            }

            var existingApplication = _applicationService.List()
                .FirstOrDefault(x =>x.ClientId == model.ClientId && x.Name.ToLower().Equals(model.AppName.ToLower()) && x.IsActive == true);

            string applicationId;
            string applicationName;

            if (existingApplication == null)
            {
                var applicationModel = new ApplicationModel()
                {
                    ClientId = model.ClientId,
                    Name = model.AppName,
                    DateCreated = DateTime.UtcNow
                };
                var newApplication = _applicationService.Create(applicationModel).Result;
                applicationId = newApplication.ApplicationId;
                applicationName = newApplication.Name;
            }
            else
            {
                applicationId = existingApplication.ApplicationId;
                applicationName = existingApplication.Name;
            }

            var price = (model.Service == ServiceCode.Sms) ? _priceSettings.Value.SmsPrice : _priceSettings.Value.EmailPrice;
            var destinations = model.To.Split(',');
            var cost = destinations.Length * price;

            

            if (client.Balance < cost)
            {
                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.InsufficientCredit,
                    Message = "Insufficient credit"
                };
            }

            gateway.Password = _generator.Decrypt(gateway.Password).Result;

            try
            {
                //var gatewayResponse = "local testing";

                var gatewayResponse = _messageAdapter.Send(model, gateway);


                client.Balance = client.Balance - cost;
                _clientService.Update(client);

                var request = new RequestModel()
                {
                    ApplicationId = applicationId,
                    GatewayId = gateway.GatewayId,
                    Recipients = destinations.Length,
                    Cost = cost,
                    Response = gatewayResponse,
                    DateCreated = DateTime.UtcNow
                };

                var lastRequest = _requestService.Create(request).Result;

                return new MessageResponseModel()
                {
                    Error = Data.Enums.MessageStatus.MessageSubmitted,
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
                Error = Data.Enums.MessageStatus.InternalServerError,
                Message = "Internal server error"
            };
            return response;
        }
    }
}

