using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace JPDictBackend
{
    public static class GetFormattedNews
    {
        [FunctionName("GetFormattedNews")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string uri = req.Query["url"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            uri = uri ?? data?.name;
            if(uri!=null)
            {
                if (!uri.Contains("www3.nhk.or.jp/news/html"))
                    return new BadRequestObjectResult("The specific uri is not a valid NHK News uri");
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(uri);
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await response.Content.ReadAsStringAsync());
                var title = html.DocumentNode.SelectSingleNode("//title").InnerText.Replace(" | NHKニュース", "");
                var img = html.DocumentNode.SelectSingleNode("//meta[@property='og:image']").Attributes["content"].Value;
                var details = html.DocumentNode.Descendants().CssSelect(".module.module--detail").First().CssSelect(".module--content");
                StringBuilder sb = new StringBuilder();
                StringBuilder addSb = new StringBuilder();
                string moreTxt = "";
                IEnumerable<HtmlNode> moreEle = new List<HtmlNode>();
                IEnumerable<HtmlNode> bodyEle;
                foreach (var d in details)
                {
                    bodyEle = d.CssSelect("#news_textbody");
                    if(d.CssSelect("#news_textmore")!=null&& d.CssSelect("#news_textmore").Count()!=0)
                    {
                        moreEle = d.CssSelect("#news_textmore");
                    }
                    var addEle = d.CssSelect(".news_add");
                    if (bodyEle?.Count() != 0 && moreEle?.Count() != 0)
                    {
                        sb.AppendLine(bodyEle.First().InnerHtml.Trim().Replace("<br><br>", "\n").Replace("<br>", "\n"));
                        moreTxt = moreEle.First().InnerHtml.Trim().Replace("<br><br>", "\n").Replace("<br>", "\n");
                    }
                    if (addEle.Count() != 0)
                    {
                        foreach(var i in addEle)
                        {
                            var h3 = i.SelectNodes("h3")?.FirstOrDefault();
                            addSb.Append(h3?.InnerText.Trim()+"\n");
                            var divs = i.SelectNodes("div");
                            if(divs.Count!=0)
                            {
                                var div = divs.OrderByDescending(a => a?.InnerText.Length).FirstOrDefault();
                                if (moreEle.Count() != 0 && moreTxt == div.InnerHtml.Trim().Replace("<br><br>", "\n").Replace("<br>", "\n"))
                                {
                                    moreTxt = "";
                                }
                                addSb.AppendLine(div.InnerHtml.Trim().Replace("<br><br>", "\n").Replace("<br>","\n"));
                            }
                            
                        }
                        
                    }
                }
                sb.Append(moreTxt);
                sb.AppendLine(addSb.ToString());
                return (ActionResult)new OkObjectResult(new { Title = title, Content = sb.ToString().Replace("\r", "\n"), Image = img });
            }
            
            return new BadRequestObjectResult("Please pass a url on the query string or in the request body");
        }
    }
}
