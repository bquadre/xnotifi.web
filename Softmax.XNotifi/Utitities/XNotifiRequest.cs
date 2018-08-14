using System.Net;

namespace Softmax.XNotifi.Utitities
{
    public static class XNotifiRequest
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

        
    }
}
