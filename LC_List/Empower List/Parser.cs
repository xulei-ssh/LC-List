using System;
using System.Collections.Generic;
using System.Xml;
using System.IO.Compression;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using WordPlugin;

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
                pi.ProductName = p.Attributes["product"].InnerText;
                pi.Items = new List<Item>();
                foreach (XmlNode i in p.ChildNodes)
                {
                    if (i.NodeType == XmlNodeType.Element)
                    {
                        Item item = new Item
                        {
                            Name = i.Attributes["exp"].InnerText,
                            LCCondition = int.Parse(i.Attributes["condition"].InnerText),
                            Injs = new ObservableCollection<Inj>()
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
                s += "<Project name=\"" + p.Key + "\" product=\"" + p.Value.ProductName + "\" protocol=\"" + p.Value.Protocol + "\">";
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
                if (u.Name == "root")
                {
                    s += "<Entry name=\"root\" group=\"root\" authType=\"0\" token=\"\" status=\"enabled\"/>";
                }
                else
                {
                    s += "<Entry name=\"" + u.Name + "\" group=\"" + u.Group.ToString() + "\" authType=\"" + u.AuthType.ToString() + "\" token=\"" + u.Token + "\" status=\"" + u.Status.ToString() + "\"/>";
                }
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
            string tempFileLocation = location + ".temp";
            byte[] arr1 = new byte[(int)stream.Length];
            stream.Read(arr1, 0, (int)stream.Length);
            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = (byte)(arr1[i] + i);
            }
            FileStream ms1 = File.Create(tempFileLocation);
            GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);
            gz.Write(arr1, 0, arr1.Length);
            gz.Close();
            ms1.Close();
            File.Move(location, location.Replace("ds", "Backup\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString()));
            File.Move(tempFileLocation, location);
        }
        public static bool SaveList(string proj, List<string> itemsDone, List<Item> items, Dictionary<int, string> stdSuffix, List<ObservableCollection<ListItem>> lists, List<string> lots, string fileName)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    string s = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ListDocument>";
                    s += "<Proj name=\"" + proj + "\"/><ItemList>";
                    foreach (var i in itemsDone)
                    {
                        s += "<Item name=\"" + i + "\"/>";
                    }
                    s += "</ItemList><STD>";
                    Dictionary<int, string> stdType = new Dictionary<int, string>();      //STD号，项目名
                    foreach (var i in items)
                    {
                        if (itemsDone.Contains(i.Name) && i.StdType != 0 && !stdType.ContainsKey(i.StdType))
                        {
                            stdType.Add(i.StdType, i.Name);
                        }
                    }
                    if (stdSuffix.Count != 0)
                    {
                        foreach (var i in stdSuffix)
                        {
                            s += "<Entry suf=\"" + stdSuffix[i.Key] + "\" item=\"" + stdType[i.Key] + "\"/>";
                        }
                    }
                    s += "</STD><List>";
                    foreach (var i in lists)
                    {
                        foreach (var j in i)
                        {
                            //设计溶出、含量均匀度、耐酸力的格式，仅第一针
                            string dissolutionIgnoredPattern = @"^*\-R[2-6]$";                               //所有 溶出的 非 R1结尾的
                            string contentUniformityIgnoredPattern = @"^*\-HJ([0-9]{0,1}[02-9]$)";          //所有 含量均匀度的 非 1\21\31...结尾的
                            string acidIgnoredPattern = @"^*\-N[2-6]$";                                     //所有 耐酸力的 非 R1结尾的
                            if (!(Regex.IsMatch(j.Name, dissolutionIgnoredPattern) || Regex.IsMatch(j.Name, contentUniformityIgnoredPattern) || Regex.IsMatch(j.Name, acidIgnoredPattern)))
                            {
                                s += "<Entry count=\"" + j.Count + "\" name=\"" + j.Name + "\"/>";
                            }
                        }
                    }
                    s += "</List><Lots>";
                    foreach (var i in lots)
                    {
                        if (i != "") s += "<Lot l=\"" + i + "\"/>";
                    }
                    s += "</Lots></ListDocument>";
                    s = s.Replace("&","[and_placeholder]");

                    writer.Write(s);
                    writer.Flush();
                    stream.Position = 0;
                    string tempFileLocation = AppDomain.CurrentDomain.BaseDirectory + @"Sequences\" + fileName + ".elw.temp";
                    byte[] arr1 = new byte[(int)stream.Length];
                    stream.Read(arr1, 0, (int)stream.Length);
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        arr1[i] = (byte)(arr1[i] - i);
                    }
                    FileStream ms1 = File.Create(tempFileLocation);
                    GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);
                    gz.Write(arr1, 0, arr1.Length);
                    gz.Close();
                    ms1.Close();
                    File.Move(tempFileLocation, AppDomain.CurrentDomain.BaseDirectory + @"Sequences\" + fileName + ".elw");
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static void GenReport(string filename, int startIndex,string fontName)
        {
            Dictionary<string, string> syn = GenSyn();
            FileStream srcFs = File.OpenRead(filename);
            XmlDocument dataFile = new XmlDocument();
            GZipStream g = new GZipStream(srcFs, CompressionMode.Decompress);
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
                eData[i] = (byte)(eData[i] + i);
            }
            ms = new MemoryStream(eData);
            dataFile.Load(ms);
            srcFs.Close();
            XmlNode root = dataFile.SelectSingleNode("ListDocument");
            var projName = root.SelectSingleNode("Proj").Attributes["name"].InnerText;
            List<string> items = new List<string>();
            foreach (XmlNode i in root.SelectSingleNode("ItemList").ChildNodes)
            {
                items.Add(i.Attributes["name"].InnerText);
            }
            List<string> lots = new List<string>();
            foreach (XmlNode i in root.SelectSingleNode("Lots").ChildNodes)
            {
                lots.Add(i.Attributes["l"].InnerText);
            }
            Dictionary<string, string> STD = new Dictionary<string, string>();
            foreach (XmlNode i in root.SelectSingleNode("STD").ChildNodes)
            {
                STD.Add(i.Attributes["suf"].InnerText, i.Attributes["item"].InnerText);
            }
            List<ListInj> injs = new List<ListInj>();
            foreach (XmlNode i in root.SelectSingleNode("List").ChildNodes)
            {
                injs.Add(new ListInj(int.Parse(i.Attributes["count"].InnerText), i.Attributes["name"].InnerText));
            }
            injs = SimplifyList(injs);
            // Below is writing docx file using items, lots, STD, injs
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            string temp = Environment.GetEnvironmentVariable("TEMP");
            Random rnd = new Random();
            string shortFileName = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 5);
            if (temp == "")
            {
                temp = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + shortFileName + "_" + rnd.Next(0,500).ToString() + ".docx";
            }
            else
            {
                temp = temp + "\\" + shortFileName + "_" + rnd.Next(0,500).ToString() + ".docx";
            }
            using (DocX document = DocX.Create(temp, DocumentTypes.Document))
            {
                //Start of title
                string title = projName;
                foreach (var i in items)
                {
                    title += syn[i] + "、";
                }
                title = title.Substring(0, title.Length - 1);
                title += "测定";
                document.InsertParagraph(title).FontSize(16d).SpacingAfter(8).Font(new Font(fontName)).Alignment = Alignment.center;
                // End of Title (OK)
                // Start of Lot
                var p = document.InsertParagraph();
                // Append some text and add formatting.
                p.Append("批号：\t").Font(new Font(fontName)).FontSize(13);
                p.Append(ParseLotReport(lots).Replace("[and_placeholder]", "&")).Font(new Font(fontName)).FontSize(13);
                //End of Lot
                //Start of Table
                var t = document.AddTable(1, 3);
                var colWidth = new float[] { 180f,700f, 150f };
                t.SetWidths(colWidth);
                t.Alignment = Alignment.center;
                t.Design = TableDesign.TableGrid;
                WordPlugin.Formatting f = new WordPlugin.Formatting();
                f.FontFamily = new Font(fontName);
                f.Size = 13;
                t.Rows[0].Cells[0].Paragraphs[0].Append("图谱编号", f);
                t.Rows[0].Cells[0].VerticalAlignment = VerticalAlignment.Center;
                t.Rows[0].Cells[1].Paragraphs[0].Append("图谱内容", f);
                t.Rows[0].Cells[1].VerticalAlignment = VerticalAlignment.Center;
                t.Rows[0].Cells[2].Paragraphs[0].Append("备注", f);
                t.Rows[0].Cells[2].VerticalAlignment = VerticalAlignment.Center;
                if (startIndex != 1)
                {
                    injs.Insert(0, new ListInj(startIndex - 1, ""));
                }
                startIndex = 1;
                foreach (var inj in injs)
                {
                    string strIndex = startIndex.ToString();
                    if (inj.Count != 1) strIndex += "-";
                    startIndex += inj.Count;
                    if (inj.Count != 1) strIndex += (startIndex - 1).ToString();
                    var c = t.InsertRow();
                    c.Cells[0].Paragraphs[0].Append(strIndex, f);
                    c.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    c.Cells[1].Paragraphs[0].Append(inj.Name == "" ? "" : ParseInjName(inj.Name).Replace("[and_placeholder]", "&"), f);
                    c.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    c.Cells[2].Paragraphs[0].Append("--", f);
                    c.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                }
                document.InsertParagraph().InsertTableAfterSelf(t);
                document.Save();
            }
            System.Diagnostics.Process.Start("winword.exe", temp);
        }
        public static Dictionary<string, string> GenSyn()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Dissolution", "溶出度");
            result.Add("Assay", "含量");
            result.Add("Content Uniformity", "含量均匀度");
            result.Add("Related Substance", "有关物质");
            result.Add("Acid Tolerance", "耐酸力");
            return result;
        }
        public static string ParseInjName(string input)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements.Add("HS1", "黄色5号");
            replacements.Add("-TimeA", "第一个时间点");
            replacements.Add("-TimeB", "第二个时间点");
            replacements.Add("-TimeC", "第三个时间点");
            replacements.Add("FLD1", "分离度1");
            replacements.Add("FLD2", "分离度2");
            replacements.Add("FLD", "分离度");
            replacements.Add("KB1", "空白");
            replacements.Add("KB", "空白");
            replacements.Add("LMD", "灵敏度");
            replacements.Add("-HJ21", "含量均匀度");
            replacements.Add("-HJ11", "含量均匀度");
            replacements.Add("-HJ1", "含量均匀度");
            replacements.Add("-HJ01", "含量均匀度");
            replacements.Add("-N1", "耐酸力");
            replacements.Add("YSY", "预试液");
            replacements.Add("FL1", "辅料");
            replacements.Add("-S", "有关物质");
            replacements.Add("TW", "拖尾");

            foreach (var rep in replacements)
            {
                if (input.Contains(rep.Key))
                {
                    return input.Replace(rep.Key, rep.Value);
                }
            }
            return input;
        }
        public static List<ListInj> SimplifyList(List<ListInj> e)
        {
            List<ListInj> result1 = new List<ListInj>();
            foreach (var i in e)
            {
                //含量，去掉H2和H3，变更injCount
                if (i.Name.EndsWith("-H1"))
                {
                    result1.Add(new ListInj((i.Name.Contains("(") || i.Name.Contains("（")) ? 3 : 2, i.Name));
                }
                else if (i.Name.Contains("-H2") || i.Name.Contains("-H3"))
                {
                    continue;
                }
                //有关物质
                else if (i.Name.EndsWith("-Y"))
                {
                    if (result1.Count != 0 && result1[result1.Count - 1].Name == i.Name.Replace("-Y", "-S"))
                    {
                        result1[result1.Count - 1].Count++;
                        continue;
                    }
                    else
                    {
                        result1.Add(new ListInj(i.Count, i.Name));
                    }
                }
                else if (result1.Count != 0 && result1[result1.Count - 1].Name.Contains("KB") && i.Name.Contains("KB"))
                {
                    result1[result1.Count - 1].Count += i.Count;
                }
                else if (result1.Count != 0 && result1[result1.Count - 1].Name.Contains("FL") && i.Name.Contains("FL") && !i.Name.Contains("FLD"))
                {
                    result1[result1.Count - 1].Count += i.Count;
                }
                else if (result1.Count != 0 && result1[result1.Count - 1].Name.Contains("HS") && i.Name.Contains("HS"))
                {
                    result1[result1.Count - 1].Count += i.Count;
                }
                else if (i.Name.EndsWith("-R1") || i.Name.EndsWith("-N1"))
                {
                    result1.Add(new ListInj(6, i.Name));
                }
                else if (i.Name.EndsWith("-HJ1") || i.Name.EndsWith("-HJ11") || i.Name.EndsWith("-HJ21"))
                {
                    result1.Add(new ListInj(10, i.Name));
                }
                else if (result1.Count != 0 && result1[result1.Count - 1].Name == i.Name)
                {
                    result1[result1.Count - 1].Count = result1[result1.Count - 1].Count + i.Count;
                }
                else
                {
                    result1.Add(new ListInj(i.Count, i.Name));
                }
            }
            result1.RemoveAll(x => x.Name.EndsWith("-2-S") || x.Name.EndsWith("-3-S"));
            for (int i = 0; i < result1.Count; i++)
            {
                if (result1[i].Name.EndsWith("-1-S"))
                {
                    result1[i].Name = result1[i].Name.Replace("-1-S", "-S");
                    result1[i].Count = 6;
                }
            }
            //进一步简化有关物质、含量
            List<ListInj> result2 = new List<ListInj>();
            ListInj prevInj = result1[0];
            result2.Add(new ListInj(result1[0].Count, result1[0].Name));
            for (int i = 1; i < result1.Count; i++)
            {
                if ((result1[i].Name.EndsWith("-S") || result1[i].Name.EndsWith("-Y")) && (result2[result2.Count - 1].Name.EndsWith("-S")) || (result2[result2.Count - 1].Name.EndsWith("-Y")))
                {
                    result2[result2.Count - 1].Name = result2[result2.Count - 1].Name.Replace("-S", "\0" + result1[i].Name.Substring(0, result1[i].Name.Length - 2) + "-S");
                    result2[result2.Count - 1].Name = result2[result2.Count - 1].Name.Replace("-Y", "\0" + result1[i].Name.Substring(0, result1[i].Name.Length - 2) + "-S");
                    result2[result2.Count - 1].Count += result1[i].Count;
                }
                else if (result1[i].Name.EndsWith("-H1") && result2[result2.Count - 1].Name.EndsWith("-H1"))
                {
                    result2[result2.Count - 1].Name = result2[result2.Count - 1].Name.Replace("-H1", "\0" + result1[i].Name.Substring(0, result1[i].Name.Length - 3) + "-H1");
                    result2[result2.Count - 1].Count += result1[i].Count;
                }
                else
                {
                    result2.Add(new ListInj(result1[i].Count, result1[i].Name));
                }               
            }
            for (int i = 0; i < result2.Count; i++)
            {
                if (result2[i].Name.EndsWith("-S") || result2[i].Name.EndsWith("-Y"))
                {
                    result2[i].Name = result2[i].Name.Substring(0, result2[i].Name.Length - 2) + "有关物质";
                }
                else if (result2[i].Name.EndsWith("-H1"))
                {
                    result2[i].Name = result2[i].Name.Substring(0, result2[i].Name.Length - 3) + "含量";
                }
                else if (result2[i].Name.EndsWith("-HJ1"))
                {
                    result2[i].Name = result2[i].Name.Substring(0, result2[i].Name.Length - 4) + "含量均匀度";
                }
                else if (result2[i].Name.EndsWith("-R1"))
                {
                    result2[i].Name = result2[i].Name.Substring(0, result2[i].Name.Length - 3) + "溶出度";
                }
                else if (result2[i].Name.EndsWith("-N1"))
                {
                    result2[i].Name = result2[i].Name.Substring(0, result2[i].Name.Length - 3) + "耐酸力";
                }
            }
            //批号合并处理
            for (int i = 0; i < result2.Count; i++)
            {
                if (result2[i].Name.Contains("有关物质"))
                {
                    result2[i].Name = SimplifyLotForTable(result2[i].Name.Substring(0, result2[i].Name.Length - 4)) + "有关物质";
                }
                else if (result2[i].Name.Contains("含量")&& !result2[i].Name.Contains("均匀度"))
                {
                    result2[i].Name = SimplifyLotForTable(result2[i].Name.Substring(0, result2[i].Name.Length - 2)) + "含量";
                }
            }
            return result2;
        }
        public static string SimplifyLotForTable(string e)
        {
            string[] splitLot = e.Split('\0');
            if (splitLot.Length == 1) return e;
            List<string> lots = new List<string>();
            lots.Add(splitLot[0].Trim());
            for (int i = 1; i < splitLot.Length; i++)
            {
                if (splitLot[i].Trim().StartsWith("(") || splitLot[i].Trim().StartsWith("（"))
                {
                    lots[lots.Count - 1] = lots[lots.Count - 1] + splitLot[i].Trim();
                }
                else
                {
                    lots.Add(splitLot[i].Trim());
                }
            }
            for (int i = 0; i < lots.Count; i++)
            {
                lots[i] = lots[i].Replace("（", "(").Replace("）", ")");
            }
            if (lots.Count == 1) return lots[0];
            for (int i = 0; i < lots.Count-1; i++)
            {
                if (lots[i].Contains("("))
                {
                    if (lots[i + 1].Contains(lots[i].Substring(lots[i].IndexOf("("), lots[i].IndexOf(")") - lots[i].IndexOf("(") + 1)))
                    {
                        lots[i] = lots[i].Substring(0, lots[i].IndexOf("("));
                    }
                }
            }
            string final = "";
            foreach (var q in lots)
            {
                final += q + " ";
            }
            return final;
        }
        public static string ParseLotReport(List<string> lots)
        {
            string result = "";
            for (int i = 0; i < lots.Count; i++)
            {
                lots[i] = lots[i].Replace("（", "(").Replace("）", ")");
            }
            if (lots.Count == 1) return lots[0];
            List<NameConfig> specifications = new List<NameConfig>();             //int:1-product;2-stability



            for (int i = 0; i < lots.Count-1; i++)
            {
                
                if (lots[i].Contains("("))
                {
                    if (lots[i + 1].Contains(lots[i].Substring(lots[i].IndexOf("("), lots[i].IndexOf(")") - lots[i].IndexOf("(") + 1)))
                    {
                        lots[i] = lots[i].Substring(0, lots[i].IndexOf("("));
                    }
                    specifications.Add(new NameConfig(lots[i], true));
                }
                else
                {
                    specifications.Add(new NameConfig(lots[i], false));
                }
            }
            specifications.Add(new NameConfig(lots[lots.Count - 1], lots[lots.Count - 1].Contains("(") ? true : false));
            result += specifications[0].Lot;
            for (int i = 1; i < specifications.Count; i++)
            {
                if (!specifications[i].IsStable && !specifications[i - 1].IsStable)
                {
                    result += " " + specifications[i].Lot;
                }
                else if ((specifications[i].IsStable && !specifications[i - 1].IsStable) || (!specifications[i].IsStable && specifications[i - 1].IsStable))
                {
                    result += "\r\t\t" + specifications[i].Lot;
                }
                else
                {   //Both stable lots
                    if (specifications[i - 1].Lot.Contains("("))
                    {
                        result += "\r\t\t";
                    }
                    if (specifications[i].Lot.Contains("("))
                    {
                        result += (result.EndsWith("\t") ? "" : " ") + specifications[i].Lot;
                    }
                    else
                    {
                        result += (result.EndsWith("\t") ? "" : " ") + specifications[i].Lot;

                    }
                }
            }
            if (result.EndsWith("\r\t\t")) result = result.Substring(0, result.Length - 3);
            return result;
        }
        public static string ParseConfigFile(string pattern)
        {

            FileStream srcFs = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"Config");
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
            string[] configs = Encoding.UTF8.GetString(eData).Split('\n');
            Dictionary<string, string> configsDict = new Dictionary<string, string>();
            foreach (var c in configs)
            {
                configsDict.Add(c.Split('=')[0].ToUpper(), c.Split('=')[1].ToUpper());
            }

            return configsDict[pattern.ToUpper()];
        }
    }
    public class ListInj
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public ListInj(int count, string name)
        {
            Count = count;
            Name = name;
        }
        public ListInj(string name) : this(1, name) { }
    }
    public class NameConfig
    {
        public string Lot { get; set; }
        public bool IsStable { get; set; }
        public NameConfig(string lot, bool isStable)
        {
            Lot = lot;
            IsStable = isStable;

        }
    }
}