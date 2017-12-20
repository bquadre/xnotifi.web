﻿using System;
using System.Net.Mail;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Models;
using Softmax.XNotifi.Utitities;

namespace Softmax.XNotifi.Data.Repositories
{
    public class MessagerAdapter : IMessageAdapter
    {
        public string SmsService(MessageRequestModel message, GatewayModel gateway)
        {
            var serviceUrl = gateway.ServiceUrl
                .Replace("#username", gateway.Username)
                .Replace("#password", gateway.Password)
                .Replace("#from", message.From)
                .Replace("#to", message.To)
                .Replace("#message", message.Message);

            return XMessagerRequest.Send(serviceUrl);
        }

        public string Emailservice(MessageRequestModel message, GatewayModel gateway)
        {
            var serviceUrl = gateway.ServiceUrl
                .Replace("#username", gateway.Username)
                .Replace("#password", gateway.Password)
                .Replace("#from", message.From)
                .Replace("#to", message.To)
                .Replace("#subject", message.Subject)
                .Replace("#text", message.Message)
                .Replace("#html", message.Message)
                .Replace("#brand", message.Brand);

            return XMessagerRequest.Send(serviceUrl);
        }
    }
}