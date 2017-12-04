using System;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Models;
using Softmax.XMessager.Utitities;

namespace Softmax.XMessager.Data.Repositories
{
    public class MessagerAdapter : IMessageAdapter
    {
        public string SmsService(MessageRequestModel message, GatewayModel gateway)
        {
            //

            var serviceUrl = gateway.ServiceUrl
                .Replace("#username", gateway.Username)
                .Replace("#password", gateway.Password)
                .Replace("#source", message.Source)
                .Replace("#destination", message.Destination)
                .Replace("#message", message.Message);

            return XMessagerRequest.SendSms(serviceUrl);
        }

        public string Emailservice(MessageRequestModel message, GatewayModel gateway)
        {
            throw new NotImplementedException();
        }
    }
}
