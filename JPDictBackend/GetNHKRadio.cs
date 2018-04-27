
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
    public static class GetNHKRadio
    {
        [FunctionName("GetNHKRadio")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string speed = req.Query["speed"];
            string index = req.Query["index"];
            string getItemsCount = req.Query["getItemsCount"];
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            speed = speed ?? data?.speed;
            index = index ?? data?.index;
            getItemsCount = getItemsCount ?? data?.getItemsCount;
            if(getItemsCount!=null)
            {
                if (bool.TryParse(getItemsCount, out bool g))
                {
                    if (g)
                        return new OkObjectResult(NHKRadioHelper.GetItemsCount());
                }
            }
            
            if (speed != "normal" && speed != "slow" && speed != "fast")
            {
                return new BadRequestObjectResult("Please specify a speed (normal, slow or fast)");
            }
            return (speed != null && index != null)
                ? (ActionResult)new OkObjectResult(NHKRadioHelper.GetJson(speed, index))
                : new BadRequestObjectResult("Please pass required parameters on the query string or in the request body");
        }
    }
}
