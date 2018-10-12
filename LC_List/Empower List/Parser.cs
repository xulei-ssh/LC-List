using System;
using System.Collections.Generic;
using System.Xml;
using System.IO.Compression;
using System.IO;
namespace Empower_List
{
    static class ConfigParser
    {
        public static string location = AppDomain.CurrentDomain.BaseDirectory + @"ds";
        public static Dictionary<string, ProjectInfo> ParseDrug()
        {
            Dictionary<string, ProjectInfo> data = new Dictionary<string, ProjectInfo>();
            XmlDocument dataFile = new XmlDocument();
            using (FileStream srcFs = File.OpenRead(location))
            {
                GZipStream g = new GZipStream(srcFs, CompressionMode.Decompress);             //compress stream
                MemoryStream ms = new MemoryStream();
                byte[] bytes = new byte[409600];
                int n;
                while ((n = g.Read(bytes, 0, bytes.Length)) > 0)
                {
                    ms.Write(bytes, 0, n);
                }
                g.Close();
                ms.Position = 0;
                dataFile.Load(ms);
                XmlNode root = dataFile.SelectSingleNode("Ew").SelectSingleNode("Drug");
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
                                Injs = new System.Collections.ObjectModel.ObservableCollection<Inj>()
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
            }
            return data;
        }
        public static List<UserInfo> ParseUser()
        {
            List<UserInfo> data = new List<UserInfo>();
            XmlDocument dataFile = new XmlDocument();
            using (FileStream srcFs = File.OpenRead(location))
            {
                GZipStream g = new GZipStream(srcFs, CompressionMode.Decompress);             //compress stream
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
                XmlNode root = dataFile.SelectSingleNode("Ew").SelectSingleNode("User");
                XmlNodeList users = root.ChildNodes;
                foreach (XmlNode p in users)
                {
                    UserInfo pi = new UserInfo();
                    pi.Name = p.Attributes["name"].InnerText;
                    pi.Group = (UserGroup)Enum.Parse(typeof(UserGroup), p.Attributes["group"].InnerText);
                    pi.AuthType = int.Parse(p.Attributes["authType"].InnerText);
                    pi.Token = p.Attributes["token"].InnerText;
                    pi.Status = (UserStatus)Enum.Parse(typeof(UserStatus), p.Attributes["status"].InnerText);
                    data.Add(pi);
                }
            }
            return data;
        }
        public static List<bool> ParseConfig()
        {
            List<bool> data = new List<bool>();
            XmlDocument dataFile = new XmlDocument();
            using (FileStream srcFs = File.OpenRead(location))
            {
                GZipStream g = new GZipStream(srcFs, CompressionMode.Decompress);             //compress stream
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
                XmlNode root = dataFile.SelectSingleNode("Ew").SelectSingleNode("Switch");
                XmlNodeList switches = root.ChildNodes;
                foreach (XmlNode p in switches)
                {
                    data.Add(p.Attributes["status"].InnerText == "1" ? true : false);
                }
            }
            return data;

        }

        public static void SaveUser(List<UserInfo> userInfos)
        {

        }
        public static void SaveConfig(List<bool> configs)
        {

        }
        public static void SaveDrug(Dictionary<string, ProjectInfo> drugs)
        {

        }
        public static void AddToken(string userName, string password)
        {

        }
    }
}
