using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JPDictBackend.Helper
{
    public static class HttpHelper
    {
        static HttpClient httpClient;
        public static async Task<string> GetStringAsync(string uri)
        {
            string response;
            using (httpClient = new HttpClient())
            {
                response = await httpClient.GetStringAsync(new Uri(uri));
            }
            return response;
        }
    }
}
