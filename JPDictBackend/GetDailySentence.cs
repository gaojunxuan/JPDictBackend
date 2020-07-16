
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using JPDictBackend.Helper;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using JPDictBackend.Model;
using Microsoft.Azure.Cosmos.Table;

namespace JPDictBackend
{
    public static class GetDailySentence
    {
        [FunctionName("GetDailySentence")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, [Table("DailySentence")]CloudTable table, ILogger log)
        {
            log.LogInformation("GetDailySentence: C# HTTP trigger function processed a request.");

            string index = req.Query["index"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            index = index ?? data?.index;
            if(index == null || index == "")
            {
                List<DailySentenceResult> results = new List<DailySentenceResult>();
                Random rand = new Random(DateTime.Today.DayOfYear);
                for(int j = 0; j < 5; j++)
                {
                    DailySentenceResult queryResult = await AzureStorageHelper.RetrieveEverydaySentenceData(rand.Next(0, 2522), table);
                    if (queryResult != null)
                        results.Add(queryResult);
                }
                return new OkObjectResult(results); 
            }
            else if(int.TryParse(index,out int i))
            {
                return new OkObjectResult(await AzureStorageHelper.RetrieveEverydaySentenceData(DateTime.UtcNow.AddHours(8).AddDays(i - 2).AddYears(-3).ToString("yyyyMMdd"),table));
            }
            else
            {
                log.LogError("Error: Invalid parameter(s) - index should be an integer");
                return new BadRequestObjectResult("Index should be an integer");
            }

        }
    }
}
