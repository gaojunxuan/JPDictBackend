using JPDictBackend.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JPDictBackend.Helper
{
    public static class DailySentenceHelper
    {
        const string URL = "http://portal.hjapi.com/v1/buluo/daily?langs=jp&publishDate=";
        public static DailySentenceResult GetJson(string datestr)
        {
            string yearinstring = datestr.Substring(0, 4);
            string newdate = datestr.Substring(4);
            int year = int.Parse(yearinstring);
            year -= 5;
            newdate = year.ToString() + newdate;
            var httpres = HttpHelper.GetStringAsync(URL + newdate);
            try
            {
                JToken jo = JObject.Parse(httpres)["data"];
                JToken sentenceitem = ((JObject)jo)[newdate];
                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;
                Regex regex = new Regex("<(a|p)(?: .+?)?>.*<\\/(a|p)>");
                return new DailySentenceResult() { Sentence = sentenceitem["sentence"].ToString(), Trans = sentenceitem["trans"].ToString(), Audio = sentenceitem["audio"].ToString(), Creator = sentenceitem["creator"].ToString(), SentencePoint = regex.Replace(sentenceitem["sentencePoint"].ToString(), "") };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DailySentenceResult() { Sentence = "尚未提供" };
            }
        }
    }
}
