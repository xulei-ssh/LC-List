#define enable_mbl_once
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Empower_List
{
    public partial class FinalList:Window
    {
        new ProjSelect Parent { get; set; }    
        ProjectInfo projInfo;
        string[] items;
        List<TaskSet> tasks;
        string projName;
        string invalid;
        bool isInListOne = true;
        int maxVial;
        public ObservableCollection<ListItem> FullList1 { get; set; }
        public ObservableCollection<ListItem> FullList2 { get; set; }
        VialCondition[][] conditionList;
        string[] Lots;
       
        ListItem preservedSTD1;
        List<int> stdTypes;
        public FinalList(ProjSelect parent, string projName, ProjectInfo projInfo, string[] items, List<TaskSet> tasks, bool isEleven, string maxVial, string invalid,string[] lots)
        {
            Parent = parent;
            InitializeComponent();
            this.projInfo = projInfo;
            this.items = items;
            this.tasks = tasks;
            this.projName = projName;
            FullList1 = new ObservableCollection<ListItem>();
            FullList2 = new ObservableCollection<ListItem>();
            std1StartVials = new Dictionary<int, int>();
            std1StartVialsSuffix = new Dictionary<int, string>();
            currentVial = 1;
            STDenum = 0;
            //this.lots = lots;

            // change lots abbr. to normal style
            this.Lots = new string[lots.Length];
            Regex reg = new Regex(@" ([L|A|T])([0-9]{1,2})(\b)");
            for (int i = 0; i < lots.Length; i++)
            {
                Match match = reg.Match(lots[i]);
                if (match.Success)
                {
                    Lots[i] = lots[i].Replace(" A", @"(40*75,")
                        .Replace(" L", @"(25*60,")
                        .Replace(" T", @"(30*65,") + "M)";
                }
                else
                {
                    Lots[i] = lots[i];
                }
            }
            //test over
            this.isEleven = isEleven;
            this.invalid = invalid;
            this.maxVial = int.Parse(maxVial);
            conditionList = new VialCondition[2][];
            conditionList[0] = new VialCondition[this.maxVial];
            conditionList[1] = new VialCondition[this.maxVial];
            for (int i = 0; i < conditionList.Length; i++)
            {
                for (int j = 0; j < conditionList[i].Length; j++)
                {
                    conditionList[i][j] = VialCondition.Available;
                }
            }
            preservedSTD1 = new ListItem();
            Gen();
            if (isInListOne)
            {
                TwoListLabel.Visibility = Visibility.Hidden;
                TwoListA.Visibility = Visibility.Hidden;
                TwoListB.Visibility = Visibility.Hidden;
            }
            #region temp code, to be removed after compeleting 2-list settings
            TwoListLabel.Visibility = Visibility.Hidden;
            TwoListA.Visibility = Visibility.Hidden;
            TwoListB.Visibility = Visibility.Hidden;
            #endregion
            finalList.ItemsSource = FullList1;

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            //Parent.Show();
            
            Hide();
            e.Cancel = true;
        }
        Dictionary<int, int> std1StartVials;
        Dictionary<int, string> std1StartVialsSuffix;
        int currentVial = 1;
        int STDenum = 0;
        bool isEleven = false;
        private void Gen()
        {          
            //获取所有分离度信息
            List<Inj> FLDs = new List<Inj>();
            foreach (var i in items)
            {
                Item it = projInfo[i];
                foreach (var inj in it)
                {
                    if (inj.Name.Contains("FLD") && FLDs.Find(x => x.Name == inj.Name)==null)
                    {
                        FLDs.Add(inj);
                    }
                }
            }
            if (FLDs.Count != 0)
            {
                foreach (var fld in FLDs)
                {
                    VialConfirm();            
                    Add(new ListItem(currentVial.ToString (), fld));
                    currentVial++;
                }
            }
            //获取有多少种STD         
            stdTypes = new List<int>();
            foreach (var q in items)
            {
                stdTypes.Add(projInfo[q].StdType);              
            }          
            stdTypes = stdTypes.Distinct().ToList();
            stdTypes.RemoveAll(x => x == 0);
            foreach (var item in items)
            {
                int index = items.ToList().FindIndex(x => x == item);
                var list = tasks.FindAll(x => x.Items[index]).ConvertAll(x => x.Lot);
                if (list.Count != 0)
                {
                    switch (item)
                    {
                        case "Related Substance":
                            GenSubListRS(projInfo[item], list);
                            break;
                        case "Assay":
                            GenSubListAssay(projInfo[item], list);
                            break;
                        default:
                            GenOthers(projInfo[item], list);
                            break;
                    }
                }
            }
            if (isEleven)
            {
                for (int i = 0; i < FullList1.Count; i++)
                {
                    if (int.TryParse(FullList1[i].Vial,out int c))
                    {
                        int currentIndex = int.Parse(FullList1[i].Vial)-1;
                        int colPerPlate = 6;
                        int rowPerPlate = 11;
                        int n1 = (currentIndex / (colPerPlate * rowPerPlate));                            //假设1个盘为11*6
                        int n2 = n1 == 0 ? currentIndex / rowPerPlate : (currentIndex - colPerPlate * rowPerPlate) / rowPerPlate ;
                        int n3 = currentIndex - n1  * colPerPlate * rowPerPlate - n2  * rowPerPlate;
                        FullList1[i].Vial = (n1+1).ToString() + ":" + ((char)(n2 + 65)).ToString() + "," + (n3+1).ToString();
                    }
                }
            }

        }
        private void GenSubListRS(Item item, List<string> lots)
        {
            if (item.NewLine)
            {
                NewLineCurrentVial(isEleven ? 11 : 10);
            }
            foreach (var inj in item.FindAll(false))
            {
                if (!inj.Name.Contains("FLD"))
                {
                    VialConfirm();
                    Add(new ListItem(currentVial.ToString(), inj));            
                    currentVial++;
                }
            }
            var sampleInjs = item.FindAll(true);
            foreach (var lot in lots)
            {
                if (sampleInjs.Count == 2)
                {
                    if (projName == "MBL-ONCE")
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            VialConfirm();
                            Add(new ListItem(currentVial.ToString (), sampleInjs[0], lot + "-" + (j + 1).ToString() + "-S"));
                            currentVial++;
                            VialConfirm();
                            Add(new ListItem(currentVial.ToString (), sampleInjs[0], lot + "-" + (j + 1).ToString() + "-Y"));
                            currentVial++;
                        }
                    }
                    else
                    {
                        VialConfirm();
                        Add(new ListItem(currentVial.ToString (), sampleInjs[0], lot + "-S"));
                        currentVial++;
                        VialConfirm();
                        Add(new ListItem(currentVial.ToString (), sampleInjs[1], lot + "-Y"));
                        currentVial++;
                    }
                }
                else
                {
                    VialConfirm();
                    Add(new ListItem(currentVial.ToString (), sampleInjs[0], lot + "-Y"));
                    currentVial++;
                }
            }
        }
        private void GenSubListAssay(Item item, List<string> lots)
        {
            if (item.NewLine)
            {
                NewLineCurrentVial(isEleven ? 11 : 10);
            }
            bool writeSTD = false;
            ListItem std1 = new ListItem();
            foreach (var inj in item.FindAll(false))
            {
                if (inj.Name.Contains("STD1"))
                {
                    //已有STD1
                    if (std1StartVials.ContainsKey(item.StdType))
                    {
                        writeSTD = false;
                        if (stdTypes.Count > 1)
                        {
                            std1 = new ListItem(std1StartVials[item.StdType].ToString(), inj, inj.Name + std1StartVialsSuffix[item.StdType]);
                        }
                        else
                        {
                            std1 = new ListItem(std1StartVials[item.StdType].ToString(), inj);
                        }
                    }   
                    //无STD1
                    else
                    {
                        writeSTD = true;
                        if (stdTypes.Count > 1)
                        {
                            VialConfirm();
                            std1 = new ListItem(currentVial.ToString(), inj, inj.Name + "-" + (char)(STDenum + 65));
                            Add(std1);
                            std1StartVials.Add(item.StdType, currentVial);
                            std1StartVialsSuffix.Add(item.StdType, "-" + (char)(STDenum + 65));
                            STDenum++;
                            currentVial++;
                        }
                        else
                        {
                            VialConfirm();
                            std1 = new ListItem(currentVial.ToString(), inj);
                            Add(std1);
                            std1StartVials.Add(item.StdType, currentVial);
                            currentVial++;
                        }
                    }
                }
                else if (inj.Name.Contains("STD2") || inj.Name.Contains("STD3"))
                {
                    if (writeSTD)
                    {
                        VialConfirm();
                        Add(new ListItem(currentVial.ToString(), inj, FullList1.Last().Name.Replace(FullList1.Last().Name.Contains("STD1") ? "STD1" : "STD2", FullList1.Last().Name.Contains("STD1") ? "STD2" : "STD3")));
                        currentVial++;
                    }
                }
                else if (!inj.Name.Contains("FLD"))
                {
                    VialConfirm();
                    Add(new ListItem(currentVial.ToString(), inj));
                    currentVial++;
                }
            }
            std1 = std1.InsertSTD1();                               //将特殊的std1(5针)转变为插针(1针)
            var sampleInjs = item.FindAll(true);
            int assayEnum = 0;
            foreach (var lot in lots)
            {
                if ((assayEnum >= 9)||(assayEnum==8&&(lot.Contains ("(")||lot.Contains ("（"))))
                {
                    Add(std1);
                    assayEnum = 0;
                }
                VialConfirm();
                Add(new ListItem(currentVial.ToString (), item.Injs.Last(), lot + "-H1"));
                currentVial++;
                VialConfirm();
                Add(new ListItem(currentVial.ToString (), item.Injs.Last(), lot + "-H2"));
                currentVial++;
                assayEnum += 2;
                if (lot.Contains("(") || lot.Contains("（"))
                {
                    VialConfirm();
                    Add(new ListItem(currentVial.ToString(), item.Injs.Last(), lot + "-H3"));
                    currentVial++;
                    assayEnum++;
                }
            }
            Add(std1);
        }
        private void GenOthers(Item item, List<string> lots)
        {
            bool writeSTD = false;
            ListItem std1 = new ListItem();
            //speical injs process
            //CDN 溶出的speical序列特殊化
            bool specialInsert = false;
            if (projName == "CDN" && item.Name == "Dissolution" && item.Injs.First().Volume > 100)
            {
                //Hint 1
                //如果选择了新列，则需要特殊插针
                if (item.NewLine)
                {
                    //先搜索是否有符合条件的连续Available，记录第一个Avail瓶号；如没有，搜索紧靠前面是否有符合条件的连续Preserved，记录第一个Avail瓶号；如都没有，则使用currentVial
                    int availVial = 0;
                    if ((FindAvailable(4, out availVial) || FindPreserved(4, out availVial)) && VialConfirm(availVial))
                    {
                        for (int i = 0; i < 4; i++)
                        {                          
                            Add(new ListItem((availVial + i).ToString(), item.Injs[i]));
                            if (i == 0) std1 = new ListItem((availVial + i).ToString(), item.Injs[i]);
                            conditionList[isInListOne ? 0 : 1][availVial - 1 + i] = VialCondition.Used;
                        }
                        specialInsert = true;
                    }
                }
                if (!specialInsert)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        VialConfirm();
                        Add(new ListItem(currentVial.ToString(), item.Injs[i]));
                        if (i == 0) std1 = new ListItem(currentVial.ToString(), item.Injs[i]);
                        currentVial++;
                    }
                }
            }
            else
            {
                //获取非FLD特殊针
                var specials = item.FindAll(false).FindAll(x => !x.Name.Contains("FLD"));
                //获取STD针数
                int countSTD = specials.Count(x => x.Name.Contains("STD"));
                //如果STD存在，则设置插针STD，然后设置非STD的特殊针；不存在则继续
                if (std1StartVials.ContainsKey(item.StdType))
                {
                    var currentSTD1Condition = specials.Find(x => x.Name.Contains("STD1"));
                    if (stdTypes.Count > 1)
                    {
                        std1 = new ListItem(std1StartVials[item.StdType].ToString(), currentSTD1Condition, currentSTD1Condition.Name + std1StartVialsSuffix[item.StdType]).InsertSTD1();
                    }
                    else
                    {
                        std1 = new ListItem(std1StartVials[item.StdType].ToString(), currentSTD1Condition).InsertSTD1();
                    }
                    if (item.NewLine && specials.Count != countSTD)
                    {
                        //先搜索是否有符合条件的连续Available，记录第一个Avail瓶号；如没有，搜索紧靠前面是否有符合条件的连续Preserved，记录第一个Avail瓶号；如都没有，则使用currentVial
                        int availVial = 0;
                        if ((FindAvailable(specials.Count - countSTD, out availVial) || FindPreserved(specials.Count - countSTD, out availVial)) && VialConfirm(availVial))
                        {
                            for (int i = 0; i < specials.Count - countSTD; i++)
                            {
                                Add(new ListItem((availVial + i).ToString(), item.Injs[i]));
                                conditionList[isInListOne ? 0 : 1][availVial - 1 + i] = VialCondition.Used;
                            }
                            specialInsert = true;
                        }
                    }
                }
                else
                {
                    int count = item.FindAll(false).Count(x => !x.Name.Contains("FLD"));
                    if (item.NewLine)
                    {
                        //先搜索是否有符合条件的连续Available，记录第一个Avail瓶号；如没有，搜索紧靠前面是否有符合条件的连续Preserved，记录第一个Avail瓶号；如都没有，则使用currentVial
                        int availVial = 0;
                        if ((FindAvailable(count, out availVial) || FindPreserved(count, out availVial)) && VialConfirm(availVial))
                        {
                            int increment = 0;
                            foreach(var inj in item.FindAll(false ).Where(x=>!x.Name.Contains ("FLD")))
                            {
                                if (inj.Name.Contains("STD1") && stdTypes.Count > 1)
                                {
                                    writeSTD = true;
                                    Add(new ListItem((availVial + increment).ToString(), inj, inj.Name + "-" + (char)(STDenum + 65)));
                                    std1 = new ListItem((availVial + increment).ToString(), inj, inj.Name + "-" + (char)(STDenum + 65));
                                    std1StartVials.Add(item.StdType, currentVial);
                                    std1StartVialsSuffix.Add(item.StdType, "-" + (char)(STDenum + 65));
                                    STDenum++;
                                    conditionList[isInListOne ? 0 : 1][availVial - 1 + increment] = VialCondition.Used;
                                }
                                else
                                {
                                    if (writeSTD && (inj.Name.Contains("STD2") || (inj.Name.Contains("STD3"))))
                                    {
                                        string name = inj.Name + std1StartVialsSuffix[item.StdType];
                                        Add(new ListItem((availVial + increment).ToString(), inj, name));
                                    }
                                    else
                                    {
                                        Add(new ListItem((availVial + increment).ToString(), inj));
                                    }
                                    conditionList[isInListOne ? 0 : 1][availVial - 1 + increment] = VialCondition.Used;
                                }
                                increment++;
                            }
                            specialInsert = true;
                        }
                    }
                    if (!specialInsert)
                    {
                        foreach (var inj in item.FindAll(false))
                        {
                            if (!inj.Name.Contains("FLD"))
                            {
                                if (inj.Name.Contains("STD1"))
                                {
                                    writeSTD = true;
                                    if (stdTypes.Count > 1)
                                    {
                                        VialConfirm();
                                        std1 = new ListItem(currentVial.ToString(), inj, inj.Name + "-" + (char)(STDenum + 65));
                                        Add(std1);
                                        std1StartVials.Add(item.StdType, currentVial);
                                        std1StartVialsSuffix.Add(item.StdType, "-" + (char)(STDenum + 65));
                                        STDenum++;
                                        currentVial++;
                                    }
                                    else
                                    {
                                        VialConfirm();
                                        std1 = new ListItem(currentVial.ToString(), inj);
                                        Add(std1);
                                        std1StartVials.Add(item.StdType, currentVial);
                                        currentVial++;
                                    }
                                }
                                else if (inj.Name.Contains("STD2") || inj.Name.Contains("STD3"))
                                {
                                    if (writeSTD)
                                    {
                                        VialConfirm();
                                        Add(new ListItem(currentVial.ToString(), inj, FullList1.Last().Name.Replace("STD2","STD3").Replace("STD1", "STD2")));
                                        currentVial++;
                                    }
                                }
                                else
                                {
                                    VialConfirm();
                                    Add(new ListItem(currentVial.ToString(), inj));
                                    currentVial++;
                                }
                            }
                        }
                    }
                    std1 = std1.InsertSTD1();
                }
            }
            //end of special inj process
            //sample inj process
            var sampleInjs = item.FindAll(true);
            foreach (var lot in lots)
            {
                if (item.NewLine)
                {
                    NewLineCurrentVial(isEleven ? 11 : 10);
                }
                switch (item.Name)
                {
                    case "Acid Tolerance":
                    case "Dissolution":
                        //MBL-ONCE 溶出特殊化
                        if (projName == "MBL-ONCE")
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                if (item.NewLine)
                                {
                                    NewLineCurrentVial(isEleven ? 11 : 10);
                                }
                                for (int i = 0; i < 6; i++)
                                {
                                    VialConfirm();
                                    Add(new ListItem(currentVial.ToString(), item.Injs.Last(), lot + "-Time" + (char)(m + 65) + "-R" + (i + 1)));
                                    currentVial++;
                                }
                                VialConfirm();
                                Add(std1.IntertSTD1ForDissolutionOfLargeInjAmount(currentVial));
                                currentVial++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                VialConfirm();
                                string suf = item.Name == "Dissolution" ? "-R" : "-N";
                                Add(new ListItem(currentVial.ToString(), item.Injs.Last(), lot + suf + (i + 1)));
                                //Add(new ListItem(currentVial.ToString(), item.Injs.Last(), lot + suf + (i + 1)));
                                currentVial++;
                            }
                            if (!item.NewStd)
                            {
                                Add(std1);
                            }
                            else
                            {
                                VialConfirm();
                                Add(std1.IntertSTD1ForDissolutionOfLargeInjAmount(currentVial));
                                currentVial++;
                            }
                        }
                        break;
                    case "Content Uniformity":
                        for (int i = 0; i < 10; i++)
                        {
                            VialConfirm();
                            Add(new ListItem(currentVial.ToString(), item.Injs.Last(), lot + "-HJ" + (i + 1).ToString ("D2")));
                            currentVial++;
                        }
                        if (!item.NewStd)
                        {
                            Add(std1);
                        }
                        else
                        {
                            VialConfirm();
                            Add(std1.IntertSTD1ForDissolutionOfLargeInjAmount(currentVial));
                            currentVial++;
                        }
                        break;
                }
            }
            //end of sample inj process
        }
        private void VialConfirm()
        {
            if (currentVial > maxVial)
            {              
                isInListOne = false;
                currentVial = 1;
            }
            while (currentVial == ParseInvalid(invalid))
            {
                currentVial++;
            }
            conditionList[isInListOne ? 0 : 1][currentVial - 1] = VialCondition.Used;
        }
        private bool VialConfirm(int vial)
        {
            if (vial >= maxVial || vial == ParseInvalid(invalid) - 1)
            {
                return false;
            }
            return true;
        }
        public void NewLineCurrentVial(int baseNumber)
        {
            while (currentVial % baseNumber != 1)
            {
                conditionList[isInListOne ? 0 : 1][currentVial - 1] = VialCondition.Preserved;
                currentVial++;
                if (currentVial > maxVial)
                {
                    currentVial = 1;
                    isInListOne = false;
                }
            }
        }
        private void btnCopyA_Click(object sender, RoutedEventArgs e)
        {
            string data = "";
            foreach (var p in FullList1)
            {
                data += p.Vial + "\t" + p.Vol + "\t" + p.Count + "\t" + p.Name + "\r\n";
            }
            if (data.Length == 0) return;
            data = data.Substring(0, data.Length - 2);
            lblCopy.Content = "已复制：前四列";

            Clipboard.SetData(DataFormats.Text, data);
        }
        private void btnCopyC_Click(object sender, RoutedEventArgs e)
        {
            string data = "";
            foreach (var p in FullList1)
            {
                data += p.Vial + "\t" + p.Vol + "\t" + p.Count + "\t" + p.Name + "\t" + p.Time + "\r\n";
            }
            if (data.Length == 0) return;
            data = data.Substring(0, data.Length - 2);
            lblCopy.Content = "已复制：全部";

            Clipboard.SetData(DataFormats.Text, data);

        }
        private void TwoListA_Checked(object sender, RoutedEventArgs e)
        {
            finalList.ItemsSource = FullList1;
        }
        private void TwoListB_Checked(object sender, RoutedEventArgs e)
        {
            finalList.ItemsSource = FullList2;
        }
        private void Add(ListItem item)
        {
            if (isInListOne)
            {
                FullList1.Add(item);
            }
            else
            {
                FullList2.Add(item);
            }
        }
        private int ParseInvalid(string exp)
        {
            if (exp == "") return 0;
            if (int.TryParse(exp, out int temp)) return int.Parse(exp);
            string[] splits = exp.Split(',', ':');
            int plateAmp = (int.Parse(splits[0]) - 1) * 66;
            int colAmp = (Convert.ToInt32(splits[1].ToUpper()[0]) - 65) * 11;
            int rowAmp = int.Parse(splits[2]);
            return plateAmp + colAmp + rowAmp;
        }
        private bool FindAvailable(int count, out int vial)                     //Vial从1开始
        {
            vial = 0;
            //限定范围：寻找当前FinalList中最大瓶号以内的
            var currentList = isInListOne ? FullList1 : FullList2;
            if (currentList.Count == 0) return false;
            int maxUsedVial = currentList.Max(x => int.Parse(x.Vial));
            if (maxUsedVial < count) return false;
            for (int i = 0; i < maxUsedVial - count + 1; i++)
            {
                bool avail = true;
                for (int j = 0; j < count; j++)
                {
                    if (conditionList[isInListOne ? 0 : 1][i + j] == VialCondition.Used)
                    {
                        avail = false;
                        break;
                    }
                }
                if(avail)
                {
                    vial = i + 1;
                    return true;
                }
            }
            return false;        
        }
        private bool FindPreserved(int count, out int vial)                     //Vial从1开始
        {
            vial = 0;
            var currentList = isInListOne ? FullList1 : FullList2;
            //获取最大瓶号，表示成0开始的int
            if (currentList.Count == 0) return false;
            int maxUsedOrPreservedVial = (currentList.Max(x => int.Parse(x.Vial)) / (isEleven ? 11 : 10) + 1) * (isEleven ? 11 : 10) - 1;
            if (maxUsedOrPreservedVial < count) return false;
            for (int i = maxUsedOrPreservedVial - count + 1; i >= 0; i--)
            {
                bool avail = true;
                for (int j = 0; j < count; j++)
                {
                    if (conditionList[isInListOne ? 0 : 1][i + j] == VialCondition.Used || conditionList[isInListOne ? 0 : 1][i + j] == VialCondition.Available)
                    {
                        avail = false;
                        break;
                    }
                }
                if (avail)
                {
                    vial = i + 1;
                    return true;
                }
            }
            return false;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<ObservableCollection<ListItem>> lists = new List<ObservableCollection<ListItem>>();
            lists.Add(FullList1);
            if (FullList2.Count != 0) lists.Add(FullList2);

            SaveOptions so = new SaveOptions(this, projInfo.ProductName, items.ToList(), projInfo.Items, std1StartVialsSuffix, lists, Lots.ToList());
            so.Show();
            IsEnabled = false;
            Hide();
        }
        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader != null)
            {
                string data = "";
                switch (columnHeader.DisplayIndex)
                {
                    case 0:
                        foreach (var p in FullList1)
                        {
                            data += p.Vial + "\r\n";
                        }
                        lblCopy.Content = "已复制：瓶号";
                        break;
                    case 1:
                        foreach (var p in FullList1)
                        {
                            data += p.Vol + "\r\n";

                        }
                        lblCopy.Content = "已复制：进样量";

                        break;
                    case 2:
                        foreach (var p in FullList1)
                        {
                            data += p.Count + "\r\n";
                        }
                        lblCopy.Content = "已复制：进样次数";

                        break;
                    case 3:
                        foreach (var p in FullList1)
                        {
                            data += p.Name + "\r\n";
                        }
                        lblCopy.Content = "已复制：样品名称";

                        break;
                    case 4:
                        foreach (var p in FullList1)
                        {
                            data += p.Time + "\r\n";
                        }
                        lblCopy.Content = "已复制：时间";

                        break;
                }
                if (data.Length == 0) return;
                data = data.Substring(0, data.Length - 2);
                Clipboard.SetData(DataFormats.Text, data);
            }
        }

        private void finalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            finalList.SelectedItem = null;
            finalList.SelectedIndex = -1;
        }
    }
    public class ListItem
    {
        public string Vial { get; set; }
        public int Vol { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public double Time { get; set; }
        public ListItem(string vial,int vol,int count,string name,double time)
        {
            Vial = vial;Vol = vol;Count = count;Name = name;Time = time;
        }
        public ListItem(string vial, Inj inj)
        {
            Vial = vial;
            Vol = inj.Volume;
            Count = inj.Count;
            Name = inj.Name;
            Time = inj.Time;
        }
        public ListItem(string vial, Inj inj, string name)
        {
            Vial = vial;
            Vol = inj.Volume;
            Count = inj.Count;
            Name = name;
            Time = inj.Time;


        }
        public ListItem() { }
        public ListItem InsertSTD1()
        {
            return new ListItem(Vial, Vol, 1, Name, Time);
        }
        public ListItem IntertSTD1ForDissolutionOfLargeInjAmount(int currentVial)
        {
            return new ListItem(currentVial.ToString (), Vol, 1, Name, Time);
        }
    }
    public enum VialCondition
    {
        Available,                          //空位，可用作插针
        Preserved,                          //空位，不可用作插针
        Used                                //已使用位

    }
}
