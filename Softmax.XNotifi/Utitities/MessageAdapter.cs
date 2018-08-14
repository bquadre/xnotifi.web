using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Utitities
{
    public class MessagerAdapter : IMessageAdapter
    {
        public string Send(MessageRequestModel message, GatewayModel gateway)
        {
            var serviceUrl = gateway.ServiceUrl
                .Replace("#username", gateway.Username)
                .Replace("#password", gateway.Password)
                .Replace("#from", message.From)
                .Replace("#to", message.To)
                .Replace("#subject", message.Subject)
                .Replace("#message", message.Message)
                .Replace("#brand", message.Brand);

            return XNotifiRequest.Send(serviceUrl);
        }
    }
}
