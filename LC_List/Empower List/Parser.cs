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
            XmlNodeList projects = ParseBasic().SelectSingleNode("Drug").ChildNodes;
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
            return data;
        }
        public static List<UserInfo> ParseUser()
        {
            List<UserInfo> data = new List<UserInfo>();
            XmlNodeList users = ParseBasic().SelectSingleNode("User").ChildNodes;
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
            return data;
        }
        public static List<bool> ParseConfig()
        {
            List<bool> data = new List<bool>();
            XmlNodeList switches = ParseBasic().SelectSingleNode("Switch").ChildNodes;
            foreach (XmlNode p in switches)
            {
                data.Add(p.Attributes["status"].InnerText == "1" ? true : false);
            }
            return data;

        }
        private static XmlNode ParseBasic()
        {
            FileStream srcFs = File.OpenRead(location);
            XmlDocument dataFile = new XmlDocument();
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
            byte[] eData = ms.ToArray();
            for (int i = 0; i < eData.Length; i++)
            {
                eData[i] = (byte)(eData[i] - i);
            }
            ms = new MemoryStream(eData);
            dataFile.Load(ms);
            srcFs.Close();
            return dataFile.SelectSingleNode("Ew");
        }
        public static void SaveUser(List<UserInfo> userInfos) => Save(userInfos, ParseConfig(), ParseDrug());
        public static void SaveConfig(List<bool> configs) => Save(ParseUser(), configs, ParseDrug());
        public static void SaveDrug(Dictionary<string, ProjectInfo> drugs) => Save(ParseUser(), ParseConfig(), drugs);
        public static void ChangeToken(string userName, string password)
        {
            var users = ParseUser();
            users.FindAll(x => x.Name == userName)[0].Token = password == "" ? "" : SafeHandler.Hash(password);
            SaveUser(users);
        }
        private static void Save(List<UserInfo> userInfos, List<bool> configs, Dictionary<string, ProjectInfo> drugs)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            string s = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Ew><Drug>";
            foreach (var p in drugs)
            {
                s += "<Project name=\"" + p.Key + "\" protocol=\"" + p.Value.Protocol + "\">";
                foreach (var item in p.Value.Items)
                {
                    s += "<Item exp=\"" + item.Name + "\" condition=\"" + item.LCCondition.ToString() + "\" stdType=\"" + item.StdType.ToString() + "\">";
                    foreach(var inj in item.Injs)
                    {
                        s += "<Inj vol=\"" + inj.Volume.ToString() + "\" count=\"" + inj.Count.ToString() + "\" Name=\"" + inj.Name + "\" time=\"" + inj.Time.ToString() + "\"/>";
                    }
                    s += "</Item>";
                }
                s += "</Project>";
            }
            s += "</Drug><User>";
            foreach (var u in userInfos)
            {
                s += "<Entry name=\"" + u.Name + "\" group=\"" + u.Group.ToString() + "\" authType=\"" + u.AuthType.ToString() + "\" token=\"" + u.Token + "\" status=\"" + u.Status.ToString() + "\"/>";
            }
            s += "</User><Switch>";
            foreach (var con in configs)
            {
                s += "<Entry status=\"" + (con ? 1 : 0).ToString() + "\"/>";
            }
            s += "</Switch></Ew>";


            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            string destFile = @"d:\ds";
            byte[] arr1 = new byte[(int)stream.Length];
            stream.Read(arr1, 0, (int)stream.Length);
            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = (byte)(arr1[i] + i);
            }

            FileStream ms1 = File.Create(destFile);
            GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);

            gz.Write(arr1, 0, arr1.Length);


            gz.Close();
            ms1.Close();
            File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");
            File.Copy(@"d:\ds", @"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            File.Copy(@"d:\ds", @"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");




        }
    }
}
