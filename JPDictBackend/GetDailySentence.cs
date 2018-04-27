
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using JPDictBackend.Helper;
using System;

namespace JPDictBackend
{
    public static class GetDailySentence
    {
        [FunctionName("GetDailySentence")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string index = req.Query["index"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            index = index ?? data?.index;
            if(index==null)
            {
                return new BadRequestObjectResult("Please pass required parameters on the query string or in the request body"); 
            }
            if(int.TryParse(index,out int i))
            {
                return (ActionResult)new OkObjectResult(DailySentenceHelper.GetJson(DateTime.UtcNow.AddHours(8).AddDays(i - 2).ToString("yyyyMMdd")));
            }
            else
            {
                return new BadRequestObjectResult("index should be an integer");
            }

        }
    }
}
