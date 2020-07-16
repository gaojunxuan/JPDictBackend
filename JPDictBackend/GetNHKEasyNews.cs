
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using JPDictBackend.Model;

namespace JPDictBackend
{
    public static class GetNHKEasyNews
    {
        [FunctionName("GetNHKEasyNews")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("GetNHKEasyNews: C# HTTP trigger function processed a request.");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://www3.nhk.or.jp/news/easy/top-list.json");
            JArray jArray = JArray.Parse(await response.Content.ReadAsStringAsync());
            List<NHKEasyNews> result = new List<NHKEasyNews>();
            foreach(var j in jArray)
            {
                string imgUri = new Uri((string)j["news_web_image_uri"]).AbsoluteUri;
                string newsId = (string)j["news_id"];
                result.Add(new NHKEasyNews() { Title = (string)j["title"], NewsId = newsId, ImageUri = !string.IsNullOrWhiteSpace(imgUri) ? imgUri : $"https://www3.nhk.or.jp/news/easy/{newsId}/{newsId}.jpg" });
            }
            return new OkObjectResult(result);
        }
    }
}
