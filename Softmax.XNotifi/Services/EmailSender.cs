using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XNotifi.Data.Contracts;

namespace Softmax.XNotifi.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
