using JPDictBackend.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JPDictBackend.Helper
{
    public static class NHKRadioHelper
    {
        const string URL = "http://www.nhk.or.jp/r-news/newslist.js";
        public static async Task<NHKRadioResult> GetJson(string speed, string index)
        {
            bool cast = int.TryParse(index, out int i);
            if (!cast)
                i = 0;            
            var httpres = await HttpHelper.GetStringAsync(URL);
            httpres = httpres.Substring(0, httpres.Length - 2).Substring(10);
            try
            {
                JToken jo = JObject.Parse(httpres)["news"];
                JToken newsitem = ((JArray)jo)[i];
                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;
                return new NHKRadioResult() { title = newsitem["title"].ToString(), startdate = newsitem["startdate"].ToString(), enddate = newsitem["enddate"].ToString(), soundurl = "http://www.nhk.or.jp/r-news/ondemand/mp3/" + newsitem["soundlist"]["sound_" + speed]["filename"].ToString() + ".mp3" };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new NHKRadioResult() { title = "该时段的录音尚未提供" };
            }
        }
        public static async Task<int> GetItemsCount()
        {
            var httpres = await HttpHelper.GetStringAsync(URL);
            httpres = httpres.Substring(0, httpres.Length - 2).Substring(10);
            try
            {
                JToken jo = JObject.Parse(httpres)["news"];
                return ((JArray)jo).Count;
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }
    }
}
