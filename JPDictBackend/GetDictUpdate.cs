using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using JPDictBackend.Helper;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Extensions.Logging;

namespace JPDictBackend
{
    public static class GetDictUpdate
    {
        [FunctionName("GetDictUpdate")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, [Table("UpdateDict")]CloudTable table, ILogger log)
        {
            log.LogInformation("GetDictUpdate: C# HTTP trigger function processed a request.");

            return (ActionResult)new OkObjectResult(await AzureStorageHelper.RetrieveUpdateData(table));
        }
    }
}
