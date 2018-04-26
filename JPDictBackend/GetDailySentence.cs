
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using JPDictBackend.Helper;

namespace JPDictBackend
{
    public static class GetDailySentence
    {
        [FunctionName("GetDailySentence")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string date = req.Query["date"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            date = date ?? data?.date;

            return date != null
                ? (ActionResult)new OkObjectResult(DailySentenceHelper.GetJson(date))
                : new BadRequestObjectResult("Please pass required parameters on the query string or in the request body");
        }
    }
}
