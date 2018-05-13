using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JPDictBackend.Helper
{
    public static class NHKNewsHelper
    {
        //各地のニュース
        const string URL1 = "http://www3.nhk.or.jp/news/html/toppage/xml/movlist.xml";
        //アクセスランキング
        const string URL2 = "http://www3.nhk.or.jp/news/html/toppage/xml/newlist.xml";
        public static async Task<XElement> GetXmlData()
        {
            //Get data from data source
            XElement child1 = XmlHelper.LoadXmlFromString(await HttpHelper.GetStringAsync(URL1));
            XElement child2 = XmlHelper.LoadXmlFromString(await HttpHelper.GetStringAsync(URL2));
            //将第二个新闻源的 item 加到第一个 XElement 上
            XmlHelper.AddChildFromOther(child1, child2.Elements("item"));

            child1.Descendants("item").GroupBy(i => i.Element("title").Value).Where(g => g.Count() > 1).ToList().ForEach(x => x.Skip(1).Remove());


            //获得所有 item 的集合
            List<XElement> coll = child1.Elements("item").ToList();
            //过滤没有 iconPath 的 item,删去
            coll = XmlHelper.FilterItemInCollection(coll, (e) =>
            {
                var title = e.Element("title").Value;
                if (e.Element("iconPath").Value == "" || e.Element("iconPath").IsEmpty)
                    return true;
                else if (e.Element("iconPath").Value.StartsWith("news/r/") || e.Element("imgPath").Value.Contains("news/r/"))
                    return true;
                //删除 JR 运营情报
                else if (title.Contains("運転"))
                {
                    if (title.Contains("見合") || title.Contains("再開"))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });

            //随机取出 10 个 item
            coll = GetSomeItems(coll, 10);
            //删除指定的 DOM 元素
            XmlHelper.DeleteElementByName(coll, "newFlg", "cate_group", "cate");
            //如果节点不是一 http:// 开头则加上
            XmlHelper.FilterElementValueInCollection(coll, (e) =>
            {
                if (e.Name == "videoPath")
                {
                    if (!e.Value.Contains("rtmp"))
                    {
                        return "rtmp://flv.nhk.or.jp/ondemand/flv/news/&movie=" + e.Value;
                    }
                    return null;
                }

                if (e.Value.StartsWith("http://www3r.n"))
                {
                    return e.Value.Replace("http://www3r.", "http://www3.");

                }
                if (!e.Value.StartsWith("http://"))
                {
                    return "http://www3.nhk.or.jp/news/" + e.Value;
                }
                return null;
            }, "videoPath", "link", "iconPath", "imgPath");

            var res = new XElement("data", coll);
            //res.Descendants("data").Descendants("item").GroupBy(i => i.Attribute("title").Value).Where(g => g.Count() > 1).ToList().ForEach(x => x.Skip(1).Remove());
            res.Descendants("data").SelectMany(s => s.Elements("item")
                           .GroupBy(g => g.Element("title").Value)
                           .SelectMany(m => m.Skip(1))).Remove();

            return res;

        }
        static List<XElement> GetSomeItems(List<XElement> collection, int count)
        {
            Random r = new Random();
            List<int> temp = new List<int>(count);
            List<XElement> list = new List<XElement>();
            do
            {
                int index = r.Next(0, collection.Count);
                if (temp.Contains(index)) continue;
                temp.Add(index);
                list.Add(collection[index]);
            } while (list.Count < count);
            return list;
        }
    }
}
