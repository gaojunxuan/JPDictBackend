using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JPDictBackend.Helper
{
    public static class XmlHelper
    {
        public static XElement LoadXmlFromString(string response, LoadOptions Option = LoadOptions.None) => XElement.Parse(response, Option);

        public static List<XElement> GetListByIEnumerableCollection(XElement element, string elementName) => element.Elements(elementName).ToList();

        public static void AddChildFromOther(XElement parent, IEnumerable<XElement> collection)
        {
            foreach (var item in collection)
            {
                parent.Add(item);
            }
        }

        public static List<XElement> FilterItemInCollection(IEnumerable<XElement> collection, Func<XElement, bool> filter)
        {
            List<XElement> list = new List<XElement>();
            foreach (var item in collection)
            {
                if (!filter(item)) list.Add(item);
            }
            return list;
        }

        public static void DeleteElementByName(IEnumerable<XElement> collection, params string[] elementName)
        {
            foreach (var item in collection)
            {
                if (!item.IsEmpty)
                    foreach (var name in elementName)
                    {
                        id:
                        item.Element(name).Remove();
                        if (item.Element(name) != null) goto id;
                    }
            }
        }

        public static void FilterElementValueInCollection(IEnumerable<XElement> collection, Func<XElement, string> filter, params string[] elementName)
        {
            foreach (var item in collection)
            {
                foreach (var name in elementName)
                {
                    string temp = filter(item.Element(name));
                    if (temp != null) item.Element(name).Value = temp;
                }
            }
        }
    }
}
