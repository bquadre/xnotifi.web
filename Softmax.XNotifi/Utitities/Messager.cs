using PostmarkDotNet;
using PostmarkDotNet.Legacy;
using System;
using System.IO;
using System.Net;

namespace Softmax.XNotifi.Utitities
{
    public class Messager
    {
         
            public static void SendEmail(string subject, string message, string recipient, string template)
            {
                var email = new PostmarkMessage
                {
                    From = "notification@standardbenefitng.com",
                    To = recipient
                };

                // email.Bcc = EmailRecipientSettings.Default.To;
                email.Subject = subject;
                // string filename = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/" + template);
                var filename = Path.Combine(
                       Directory.GetCurrentDirectory(), "wwwroot/templates",
                       template);
                string content = File.ReadAllText(filename);
                content = content.Replace("#NotificationContent", message);
                content = content.Replace("#date", DateTime.Today.ToString("dd-MMM-yyyy"));
                content = content.Replace("#YEAR", DateTime.Now.Year.ToString());

                email.HtmlBody = content;
                email.TextBody = content;

                var client = new PostmarkClient(EmailSettings.ServerToken);
                IAsyncResult result = client.BeginSendMessage(email);
                if (result.AsyncWaitHandle.WaitOne())
                {
                    var response = client.EndSendMessage(result);
                    //return true;
                }
            }

            public static string SendSms(string from, string to, string message)
            {
                //var appsec = EmailRecipientSettings.Default.SmsKey;
                //var username = EmailRecipientSettings.Default.SmsUsername;
                //var password = EmailRecipientSettings.Default.SmsPassword;
                //var sender = EmailRecipientSettings.Default.SmsSender;

                //var appsec = EmailRecipientSettings.Default.SmsKey;
                //var username = EmailRecipientSettings.Default.SmsUsername;
                //var password = EmailRecipientSettings.Default.SmsPassword;
                //var sender = EmailRecipientSettings.Default.SmsSender;

            string queryUrl = XWireLess(from, to, message);
                var request = HttpWebRequest.Create(queryUrl);

                var response = (HttpWebResponse)request.GetResponse();
                var dataStream = response.GetResponseStream();
                string returnString = new StreamReader(dataStream).ReadToEnd();
                return returnString;
            }

            public static string Topupxtra(string sender, string recipient, string message, string transfer = null)
            {
                var url = string.Format("http://www.topupxtra.com/api?appsec={0}&type=sms&auth1=vgg&auth2=vgg&recipients={0}&sender={1}&message={2}&transref={3}",
                      recipient, sender, message, transfer);

                return url;
            }

            public static string XWireLess(string sender, string recipient, string message)
            {
                var url = string.Format("http://panel.xwireless.net/API/WebSMS/Http/v1.0a/index.php?username=powertech&password=p0w3rt3ch&sender={0}&to={1}&message={2}&reqid=1&format=json&route_id=2",
                      sender, recipient, message);
                return url;
            }
    }
}
