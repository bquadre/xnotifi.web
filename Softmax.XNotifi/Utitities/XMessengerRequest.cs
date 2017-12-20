using System.Net;

namespace Softmax.XNotifi.Utitities
{
    public static class XMessagerRequest
    {
        public static string Send(string url)
        {
            var response = string.Empty;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(url);
            }

            return response;
        }

        //public static string SendEmail(string url)
        //{
        //    var response = string.Empty;
            

        //    return response;
        //}
    }
}
