using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace JPDictBackend
{
    public static class GetFormattedEasyNews
    {
        [FunctionName("GetFormattedEasyNews")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("GetFormattedEasyNews: C# HTTP trigger function processed a request.");

            string id = req.Query["id"];
            string img = req.Query["img"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            id = id ?? data?.id;
            img = img ?? data?.img;
            if(!string.IsNullOrWhiteSpace(id))
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync($"https://www3.nhk.or.jp/news/easy/{id}/{id}.html");
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await response.Content.ReadAsStringAsync());
                var title = html.DocumentNode.Descendants().CssSelect(".article-main__title");
                var body = html.DocumentNode.Descendants().CssSelect(".article-main__body");
                string resultHtml = @"<head>
                                    <meta charset=""UTF-8"">
                                    <script src=""https://slwsp-new-res.azureedge.net/script/jquery.min.js"">
                                    </script>
                                    <script>
                                        $(document).ready(function(){
	                                        $(document).keydown(function(event) {
	    	                                            if (event.ctrlKey==true && (event.which == '61' || event.which == '107' || event.which == '173' || event.which == '109'  || event.which == '187'  || event.which == '189'  ) ) {
		                                        event.preventDefault();
	                                             }
	                                        });

	                                        $(window).bind('mousewheel DOMMouseScroll', function (event) {
	                                               if (event.ctrlKey == true) {
		                                           event.preventDefault();
	                                               }
	                                        });
                                        });
                                    </script>
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no""/>
                                    <style>
                                        html * {font-family:""Yu Mincho""; font-size:24px; -webkit-touch-callout: none; -webkit-user-select: none; -khtml-user-select: none; -moz-user-select: none; -ms-user-select: none; user-select: none; cursor:default; -ms-content-zooming:none;}
                                        rt {font-size:12px;} 
                                        body {height:fit-content;margin-left:48px; margin-top:48px; margin-right:48px; margin-bottom:96px; background-color:#F9F5E8;}
                                        h1 {font-size:48px;}
                                        h1 ruby{font-size:48px;}
                                        .colorC {color: #0041cc;}
                                        .colorL {color: #ff7f00;}
                                        .colorN {color: #35a16b;}
                                    </style>
                                </head>";

                resultHtml += title.First().OuterHtml.ToString();
                if(!string.IsNullOrWhiteSpace(img))
                    resultHtml += $@"<img src=""{img}"" draggable=""false"" style=""margin-bottom: 24px; height: auto; max-width: 100%; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);"" onerror='this.style.display = ""none""'>";
                resultHtml += body.First().InnerHtml.ToString().Replace("<a href='javascript:void(0)'", "<a");

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StringContent(resultHtml);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return result;
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
