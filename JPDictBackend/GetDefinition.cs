
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

namespace JPDictBackend
{
    public static class GetDefinition
    {
        [FunctionName("GetDefinition")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("GetDefinition: C# HTTP trigger function processed a request.");

            string keyword = req.Query["keyword"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            keyword = keyword ?? data?.keyword;

            if(keyword != null)
            {
                var result = Helper.SqliteHelper.Query(keyword);
                return (ActionResult)new OkObjectResult(result);
            }
            else
            {
                log.LogError("Error: Missing parameter(s) - \"keyword\" is empty");
                return new BadRequestObjectResult("Pass a keyword to the query string or the request body");
            }
        }
    }
}
