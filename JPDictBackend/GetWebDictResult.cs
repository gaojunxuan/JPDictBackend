
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using JPDictBackend.Helper;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Extensions.Logging;

namespace JPDictBackend
{
    public static class GetWebDictResult
    {
        [FunctionName("GetWebDictResult")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, [Table("WebDict")]CloudTable table, ILogger log)
        {
            log.LogInformation("GetWebDictResult: C# HTTP trigger function processed a request.");

            string keyword = req.Query["keyword"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            keyword = keyword ?? data?.name;

            return keyword != null
                ? (ActionResult)new OkObjectResult(await AzureStorageHelper.QueryWebDict(table, keyword))
                : new BadRequestObjectResult("Please pass a keyword on the query string or in the request body");
        }
    }
}
