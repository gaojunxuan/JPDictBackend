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
using JPDictBackend.Model;
using System.Collections.Generic;

namespace JPDictBackend
{
    public static class GetNHKRadio
    {
        [FunctionName("GetNHKRadio")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("GetNHKRadio: C# HTTP trigger function processed a request.");

            string speed = req.Query["speed"];
            string index = req.Query["index"];
            string getItemsCount = req.Query["getItemsCount"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            speed = speed ?? data?.speed;
            index = index ?? data?.index;
            getItemsCount = getItemsCount ?? data?.getItemsCount;
            if (getItemsCount != null)
            {
                if (bool.TryParse(getItemsCount, out bool g))
                {
                    if (g)
                        return new OkObjectResult(await NHKRadioHelper.GetItemsCount());
                }
            }
            
            if (speed != "normal" && speed != "slow" && speed != "fast")
            {
                log.LogError("Error: Missing parameter(s) - \"speed\"");
                return new BadRequestObjectResult("Specify a speed (normal, slow or fast)");
            }
            List<NHKRadioResult> resultList = new List<NHKRadioResult>();
            int count = await NHKRadioHelper.GetItemsCount();
            for (int i = 0; i < count; i++)
            {
                resultList.Add(await NHKRadioHelper.GetJson(speed, i));
            }
            return (speed != null && index != null)
                ? (ActionResult)new OkObjectResult(resultList)
                : new BadRequestObjectResult("Pass required parameters to the query string or the request body");
        }
    }
}
