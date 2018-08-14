using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Utitities
{
    public  class MessageFactory : IMessageFactory
    {
        private readonly IOptions<XNotifiSettings> _xnotifiSettings;
        private readonly IGatewayService _gatewayService;
        private readonly IRequestService _requestService;
        private readonly IMessageAdapter _messageAdapter;
        private readonly IApplicationService _applicationService;
        private readonly IGenerator _generator;

        public MessageFactory(IOptions<XNotifiSettings> xnotifiSettings,
            IGatewayService gatewayService,
            IRequestService requestService,
            IMessageAdapter messageAdapter,
            IApplicationService applicationService,
            IGenerator generator)
        {
            _xnotifiSettings = xnotifiSettings;
            _gatewayService = gatewayService;
            _requestService = requestService;
            _messageAdapter = messageAdapter;
            _applicationService = applicationService;
            _generator = generator;
        }

        public void SendEmailConfirmationLink(ClientModel receiver, string code)
        {
            var subject = "Email confirmation";
            var message = "Dear " + receiver.FirstName +",";
            message += "<br/><br/>";
            message += "Thanks for registering with us. ";
            message += "Kindly click on or copy the link to the browser to confirm your email<br/><br/>";
            message += "<a href="+_xnotifiSettings.Value.BaseUrl + "/account/confirm?id=" + code +">Click Here</a>";
            message += "<br/><br/>";
            message += "Thank You<br/>";
            message += "<br/><br/>";
            message += "Regards<br/>";
            message += "Softmax Technologies";
            Sender(ServiceCode.Email, _xnotifiSettings.Value.NoReplyEmail, receiver.EmailAddress,subject, message);
        }

        public void SendEmailForgotPasswordLink(ClientModel receiver)
        {
            var subject = "Reset password";
            var message = "Dear " + receiver.FirstName + ",";
            message += "<br/><br/>";
            message += "Your recently requested for password reset. ";
            message += "Kindly click on or copy the link to the browser to reset your password<br/><br/>";
            message += "<a href=" + _xnotifiSettings.Value.BaseUrl + "/account/resetpassword?id="+ receiver.ClientId +">Click Here</a>";
            message += "<br/><br/>";
            message += "Thank You<br/>";
            message += "<br/><br/>";
            message += "Regards<br/>";
            message += "Softmax Technologies";
            Sender(ServiceCode.Email, _xnotifiSettings.Value.NoReplyEmail, receiver.EmailAddress, subject, message);
        }

        private void Sender(ServiceCode service, string from, string to, string subject, string message)
        {
            var messageRequest = new MessageRequestModel()
            {
                From = from,
                To = to,
                Message = message,
                Subject = subject,
                Brand = _xnotifiSettings.Value.Brand
            };

            var gateway = GetGateway(service);
            gateway.Password = _generator.Decrypt(gateway.Password).Result;
            var application = GetCreateApplication(_xnotifiSettings.Value.Brand, _xnotifiSettings.Value.ClientId);

            var messager = (_xnotifiSettings.Value.Development)? "Local Dev." : _messageAdapter.Send(messageRequest, gateway);

            var requestLog = new RequestModel()
            {
                ApplicationId = application.ApplicationId,
                GatewayId = gateway.GatewayId,
                Service = ServiceCode.Email,
                Recipients = 1,
                Cost = 0.00m,
                Response = messager,
                DateCreated = DateTime.UtcNow
            };
            _requestService.Create(requestLog);
        }

        private GatewayModel GetGateway(ServiceCode service)
        {
            var result = _gatewayService.List().FirstOrDefault(x => x.ServiceCode == service);
            return result;
        }

        private ApplicationModel GetCreateApplication(string appName, string clientId)
        {
            var existingApplication = _applicationService.List().FirstOrDefault(x => x.ClientId == clientId && x.Name.ToLower().Equals(appName.ToLower()));
            //return result;
            ApplicationModel model;
            if (existingApplication == null)
            {
                var applicationModel = new ApplicationModel()
                {
                    ClientId = clientId,
                    Name = appName,
                    DateCreated = DateTime.UtcNow
                };
                var newApplication = _applicationService.Create(applicationModel);
                model = newApplication.Result;
            }
            else
            {
                model = existingApplication;
            }
            return model;
        }
    }
}
