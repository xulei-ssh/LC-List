using System;
using System.Collections.Generic;
using System.Xml;

namespace LC_List
{
    static class XmlParser
    {
        public static Dictionary<string, List<Item>> Parse(string xmlFileLocation)
        {
            Dictionary<string, List<Item>> data = new Dictionary<string, List<Item>>();
            XmlDocument dataFile = new XmlDocument();
            dataFile.Load(xmlFileLocation);
            XmlNode root = dataFile.SelectSingleNode("/Drug");
            XmlNodeList projects = root.ChildNodes;
            foreach (XmlNode p in projects)
            {
                List<Item> items = new List<Item>();
                foreach (XmlNode i in p.ChildNodes)
                {
                    if (i.NodeType == XmlNodeType.Element)
                    {
                        Item item = new Item
                        {
                            Name = (ItemCategory)Enum.Parse(typeof(ItemCategory), i.Attributes["exp"].InnerText),
                            LCCondition = int.Parse(i.Attributes["condition"].InnerText),
                            Config = i.Attributes["config"].InnerText,
                            Injs = new List<Inj>()
                        };
                        int temp;
                        item.StdType = int.TryParse(i.Attributes["stdType"].InnerText, out temp) ? int.Parse(i.Attributes["stdType"].InnerText) : 0;
                        foreach (XmlNode inj in i.ChildNodes)
                        {
                            item.Injs.Add(new Inj(
                                int.Parse(inj.Attributes["vol"].InnerText),
                                int.Parse(inj.Attributes["count"].InnerText),
                                inj.Attributes["Name"].InnerText,
                                double.Parse(inj.Attributes["time"].InnerText)
                                ));
                        }
                        items.Add(item);
                    }
                }
                data.Add(p.Attributes["name"].InnerText, items);
            }
            return data;
        }
        public static Dictionary<string, string> GetProtocols(string xmlFileLocation)
        {
            Dictionary<string, string> pro = new Dictionary<string, string>();
            XmlDocument dataFile = new XmlDocument();
            dataFile.Load(xmlFileLocation);
            XmlNode root = dataFile.SelectSingleNode("/Drug");
            XmlNodeList projects = root.ChildNodes;
            foreach (XmlNode p in projects)
            {
                pro.Add(p.Attributes["name"].InnerText, p.Attributes["protocol"].InnerText);

            }


            return pro;
        }
    }
}
