using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Empower_List
{
    /// <summary>
    /// List.xaml 的交互逻辑
    /// </summary>
    public partial class FinalList:Window
    {
        public bool[] Used { get; set; }
        new ProjSelect Parent { get; set; }
        
        ProjectInfo projInfo;
        string[] items;
        List<TaskSet> tasks;
        string projName;
        int invalid;
        public List<ListItem> FullList { get; set; }
        List<int> stdTypes;
        public FinalList(ProjSelect parent, string projName, ProjectInfo projInfo, string[] items, List<TaskSet> tasks, int invalid = 0)
        {
            Parent = parent;
            InitializeComponent();
            Used = new bool[200];
            Used[invalid] = true;
            this.projInfo = projInfo;
            this.items = items;
            this.tasks = tasks;
            this.projName = projName;
            FullList = new List<ListItem>();
            std1StartVials = new Dictionary<int, int>();
            std1StartVialsSuffix = new Dictionary<int, string>();
            currentVial = 1;
            STDenum = 0;
            this.invalid = invalid;
            Gen();

            finalList.ItemsSource = FullList;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Hide();
            e.Cancel = true;
        }

        //Help methods
        Dictionary<int, int> std1StartVials;
        Dictionary<int, string> std1StartVialsSuffix;
        int currentVial = 1;
        int STDenum = 0;
        private void Gen()
        {
            //获取有多少种STD
            stdTypes = new List<int>();
            foreach (var q in items)
            {
                stdTypes.Add(projInfo[q].StdType);
            }
            
            stdTypes = stdTypes.Distinct().ToList();
            stdTypes.RemoveAll(x => x == 0);
            if (items.Contains("Related Substance"))
            {
                //Find the corresponding item number
                int index = items.ToList().FindIndex(x => x == "Related Substance");
                GenSubListRS(projInfo["Related Substance"], tasks.FindAll(x => x.Items[index]).ConvertAll(x => x.Lot));
            }
            if (items.Contains("Assay"))
            {
                //Find the corresponding item number
                int index = items.ToList().FindIndex(x => x == "Assay");
                GenSubListAssay(projInfo["Assay"], tasks.FindAll(x => x.Items[index]).ConvertAll(x => x.Lot));
            }
            var allItems = items.ToList();
            allItems.RemoveAll(x => x == "Related Substance" || x == "Assay");
            foreach (var item in allItems)
            {
                int index = items.ToList().FindIndex(x => x == item);
                GenOthers(projInfo[item], tasks.FindAll(x => x.Items[index]).ConvertAll(x => x.Lot));
            }

        }
        private void VailConfirm()
        {
            while(currentVial==invalid)
            {
                currentVial++;
            }
        }

        private void GenSubListRS(Item item, List<string> lots)
        {
            foreach (var inj in item.Injs.FindAll(x => !x.Name.Contains("sp")))
            {
                VailConfirm();
                FullList.Add(new ListItem(currentVial, inj));
                currentVial++;
            }
            var sampleInjs = item.Injs.FindAll(x => x.Name.Contains("sp"));
            foreach (var lot in lots)
            {
                if (item.Config == "self")
                {
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial,sampleInjs[0],lot+"-S"));
                    currentVial++;
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial, sampleInjs[1], lot + "-Y"));
                    currentVial++;
                }
                else
                {
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial, sampleInjs[0], lot + "-Y"));
                    currentVial++;
                }
            }
        }
        private void GenSubListAssay(Item item, List<string> lots)
        {
            bool writeSTD = false;
            ListItem std1 = new ListItem();
            foreach (var inj in item.Injs.FindAll(x => !x.Name.Contains("sp")))
            {
                if (inj.Name.Contains("STD1"))
                {
                    if (std1StartVials.ContainsKey(item.StdType))
                    {
                        writeSTD = false;
                        if (stdTypes.Count > 1)
                        {
                            std1 = new ListItem(std1StartVials[item.StdType], inj, inj.Name + std1StartVialsSuffix[item.StdType]);
                        }
                        else
                        {
                            std1 = new ListItem(std1StartVials[item.StdType], inj);
                        }
                    }
                    else
                    {
                        writeSTD = true;
                        if (stdTypes.Count > 1)
                        {
                            VailConfirm();
                            std1 = new ListItem(currentVial, inj, inj.Name + "-" + (char)(STDenum + 65));
                            FullList.Add(std1);
                            std1StartVials.Add(item.StdType, currentVial);
                            std1StartVialsSuffix.Add(item.StdType, "-" + (char)(STDenum + 65));
                            STDenum++;
                            currentVial++;
                        }
                        else
                        {
                            VailConfirm();
                            std1 = new ListItem(currentVial, inj);
                            FullList.Add(std1);
                            std1StartVials.Add(item.StdType, currentVial);
                            currentVial++;
                        }
                    }
                }
                else if (inj.Name.Contains("STD2") || inj.Name.Contains("STD3"))
                {
                    if (writeSTD)
                    {
                        VailConfirm();
                        FullList.Add(new ListItem(currentVial, inj, FullList.Last().Name.Replace('1', inj.Name.Contains("STD2") ? '2' : '3')));
                        currentVial++;
                    }
                }
                else if (inj.Name.Contains("FLD")&& FullList.Count(x => x.Name == inj.Name) > 0)
                {
                    continue;
                }
                else
                {
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial, inj));
                    currentVial++;
                }
            }
            std1 = std1.InsertSTD1();
            var sampleInjs = item.Injs.FindAll(x => x.Name.Contains("sp"));
            int assayEnum = 0;
            foreach (var lot in lots)
            {
                if ((assayEnum >= 9)||(assayEnum==8&&lot.Contains ("(")))
                {
                    FullList.Add(std1);
                    assayEnum = 0;
                }
                VailConfirm();
                FullList.Add(new ListItem(currentVial, item.Injs.Last(), lot + "-H1"));
                currentVial++;
                VailConfirm();
                FullList.Add(new ListItem(currentVial, item.Injs.Last(), lot + "-H2"));
                currentVial++;
                assayEnum += 2;
                if (lot.Contains("("))
                {
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial, item.Injs.Last(), lot + "-H3"));
                    currentVial++;
                    assayEnum += 1;
                }
            }
            FullList.Add(std1);
        }
        private void GenOthers(Item item, List<string> lots)
        {
            bool writeSTD = false;
            ListItem std1 = new ListItem();
            // CDN 溶出特殊化
            if (projName == "CDN" && item.Name == "Dissolution" && item.Injs.First().Volume > 100)
            {
                for (int i = 0; i < 4; i++)
                {
                    VailConfirm();
                    FullList.Add(new ListItem(currentVial, item.Injs[i]));
                    if (i == 0) std1 = new ListItem(currentVial, item.Injs[i]);

                    currentVial++;
                }
                
            }
            else
            {
                foreach (var inj in item.Injs.FindAll(x => !x.Name.Contains("sp")))
                {
                    if (inj.Name.Contains("STD1"))
                    {
                        if (std1StartVials.ContainsKey(item.StdType))
                        {
                            writeSTD = false;
                            if (stdTypes.Count > 1)
                            {
                                std1 = new ListItem(std1StartVials[item.StdType], inj, inj.Name + std1StartVialsSuffix[item.StdType]);
                            }
                            else
                            {
                                std1 = new ListItem(std1StartVials[item.StdType], inj);
                            }
                        }
                        else
                        {
                            writeSTD = true;
                            if (stdTypes.Count > 1)
                            {
                                VailConfirm();
                                std1 = new ListItem(currentVial, inj, inj.Name + "-" + (char)(STDenum + 65));
                                FullList.Add(std1);
                                std1StartVials.Add(item.StdType, currentVial);
                                std1StartVialsSuffix.Add(item.StdType, "-" + (char)(STDenum + 65));
                                STDenum++;
                                currentVial++;
                            }
                            else
                            {
                                VailConfirm();
                                std1 = new ListItem(currentVial, inj);
                                FullList.Add(std1);
                                std1StartVials.Add(item.StdType, currentVial);
                                currentVial++;
                            }
                        }
                    }
                    else if (inj.Name.Contains("STD2") || inj.Name.Contains("STD3"))
                    {
                        if (writeSTD)
                        {
                            VailConfirm();
                            FullList.Add(new ListItem(currentVial, inj, FullList.Last().Name.Replace('1', inj.Name.Contains("STD2") ? '2' : '3')));
                            currentVial++;
                        }
                    }
                    else if (inj.Name.Contains("FLD") && FullList.Count(x => x.Name == inj.Name) > 0)
                    {
                        continue;
                    }

                    else
                    {
                        VailConfirm();
                        FullList.Add(new ListItem(currentVial, inj));
                        currentVial++;
                    }
                }
                std1 = std1.InsertSTD1();
            }

            var sampleInjs = item.Injs.FindAll(x => x.Name.Contains("sp"));

            foreach (var lot in lots)
            {
                if (item.NewLine)
                {
                    NewLineCurrentVial();
                }
                switch (item.Name)
                {
                    case "Acid Tolerance":
                    case "Dissolution":                        
                        for (int i = 0; i < 6; i++)
                        {
                            VailConfirm();
                            FullList.Add(new ListItem(currentVial, item.Injs.Last(), lot + "-R" + (i + 1)));
                            currentVial++;
                        }
                        if (item.Injs.Last().Volume < 100)
                        {
                            FullList.Add(std1);
                        }
                        else
                        {
                            VailConfirm();
                            FullList.Add(std1.IntertSTD1ForDissolutionOver100(currentVial));
                            currentVial++;
                        }
                        break;
                    case "Content Uniformity":
                        for (int i = 0; i < 10; i++)
                        {
                            VailConfirm();
                            FullList.Add(new ListItem(currentVial, item.Injs.Last(), lot + "-HJ" + (i + 1)));
                            currentVial++;
                        }
                        FullList.Add(std1);
                        break;
                }



            }


        }
        public void NewLineCurrentVial()
        {
            while (currentVial % 10 != 1)
            {
                currentVial++;
            }
        }
    }
    public class ListItem
    {
        public int Vial { get; set; }
        public int Vol { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public double Time { get; set; }
        public ListItem(int vial,int vol,int count,string name,double time)
        {
            Vial = vial;Vol = vol;Count = count;Name = name;Time = time;
        }
        public ListItem(int vial, Inj inj)
        {
            Vial = vial;
            Vol = inj.Volume;
            Count = inj.Count;
            Name = inj.Name;
            Time = inj.Time;
        }
        public ListItem(int vial, Inj inj, string name)
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
        public ListItem IntertSTD1ForDissolutionOver100(int currentVial)
        {
            return new ListItem(currentVial, Vol, 1, Name, Time);
        }
    }
    public class SubList
    {
        public int StdType { get; set; }
        private List<ListItem> items;
        public ListItem this[int index] => items[index];
        public void ChangeSTDVial(string STDName, int newVial) => items.Find(x => x.Name == STDName).Vial = newVial;
    }
    
}
