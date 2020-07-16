
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using JPDictBackend.Helper;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JPDictBackend
{
    public static class GetNHKNews
    {
        [FunctionName("GetNHKNews")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("GetNHKNews: C# HTTP trigger function processed a request.");
            return new OkObjectResult(await NHKNewsHelper.GetXmlData());
        }
    }
}
