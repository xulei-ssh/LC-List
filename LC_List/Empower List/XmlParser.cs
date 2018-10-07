using System;
using System.Collections.Generic;
using System.Xml;
using System.IO.Compression;
using System.IO;
namespace Empower_List
{
    static class ConfigParser
    {
        public static Dictionary<string, ProjectInfo> Parse(string location)
        {
            //FileStream fs1 = File.OpenRead(@"D:\config");
            //byte[] arr1 = new byte[(int)fs1.Length];
            //fs1.Read(arr1, 0, (int)fs1.Length);

            //FileStream ms1 = File.Create(@"d:\ac");
            //GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);
            //gz.Write(arr1, 0, arr1.Length);
            //gz.Close();
            //ms1.Close();

            Dictionary<string, ProjectInfo> data = new Dictionary<string, ProjectInfo>();
            XmlDocument dataFile = new XmlDocument();
            FileStream srcFs = File.OpenRead(location);
            GZipStream g = new GZipStream(srcFs,CompressionMode.Decompress);             //compress stream
            MemoryStream ms = new MemoryStream();
            byte[] bytes = new byte[40960];
            int n;
            while ((n = g.Read(bytes, 0, bytes.Length)) > 0)
            {
                ms.Write(bytes, 0, n);
            }
            g.Close();
            ms.Position = 0;
            dataFile.Load(ms);
            XmlNode root = dataFile.SelectSingleNode("/Drug");
            XmlNodeList projects = root.ChildNodes;
            foreach (XmlNode p in projects)
            {
                ProjectInfo pi = new ProjectInfo();
                pi.Protocol = p.Attributes["protocol"].InnerText;
                pi.Items = new List<Item>();
                foreach (XmlNode i in p.ChildNodes)
                {
                    if (i.NodeType == XmlNodeType.Element)
                    {
                        Item item = new Item
                        {
                            Name = i.Attributes["exp"].InnerText,
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
                        pi.Items.Add(item);
                    }
                }
                data.Add(p.Attributes["name"].InnerText, pi);
            }
            return data;
        }
    }
}
