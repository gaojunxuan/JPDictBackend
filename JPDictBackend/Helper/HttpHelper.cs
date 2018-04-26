using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace JPDictBackend.Helper
{
    public static class HttpHelper
    {
        static HttpClient httpClient;
        public static string GetStringAsync(string uri)
        {
            string response;
            using (httpClient = new HttpClient())
            {
                response = httpClient.GetStringAsync(new Uri(uri)).Result;
            }
            return response;
        }
    }
}
