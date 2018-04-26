
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
    public static class GetNHKNews
    {
        [FunctionName("GetNHKNews")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return new OkObjectResult(NHKNewsHelper.GetXmlData());
        }
    }
}
