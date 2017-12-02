using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Softmax.XMessager.Utitities
{
    public static class XMessagerRequest
    {
        public static string SendSms(string url)
        {
            var response = string.Empty;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(url);
            }

            return response;
        }

        public static string SendEmail(string url)
        {
            var response = string.Empty;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(url);
            }

            return response;
        }
    }
}
