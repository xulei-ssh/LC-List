using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace LC_List
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, List<Item>> data;
        List<UIElement> controls;
        List<Item> currentDataset;                              //for generation
        int currentLCCondition;
        Dictionary<ItemCategory, string> itemParser;
        Dictionary<string, ItemCategory> itemReverseParser;
        List<string> usedItem;
        Dictionary<string, string> protocols;
        ItemEditor ie;
        MethodEditor me;
        public MainWindow()
        {
            InitializeComponent();
            data = XmlParser.Parse(@"D:\config.xml");
            protocols = XmlParser.GetProtocols(@"D:\config.xml");
            controls = new List<UIElement> { var1, var2, var3, var4 };
            foreach (var proj in data)
            {
                comboProj.Items.Add(proj.Key);
            }
            itemParser = new Dictionary<ItemCategory, string>();
            itemParser.Add(ItemCategory.C, "含量");
            itemParser.Add(ItemCategory.D, "溶出度");
            itemParser.Add(ItemCategory.R, "有关物质");
            itemParser.Add(ItemCategory.T, "耐酸力");
            itemParser.Add(ItemCategory.U, "含量均匀度");
            itemReverseParser = new Dictionary<string, ItemCategory>();
            itemReverseParser.Add("含量", ItemCategory.C);
            itemReverseParser.Add("溶出度", ItemCategory.D);
            itemReverseParser.Add("有关物质", ItemCategory.R);
            itemReverseParser.Add("耐酸力", ItemCategory.T);
            itemReverseParser.Add("含量均匀度", ItemCategory.U);
        }
        private void comboProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Reset all 
            setComboToBlank(-1);
            controls.ForEach(x => x.IsEnabled = false);
            currentDataset = data[(string)comboProj.SelectedItem];
            var1.IsEnabled = true;
            c1.ItemsSource = currentDataset.Select(x => x.Name).Select(x => itemParser[x]);
            usedItem = new List<string>();
            if (comboProj.SelectedIndex == -1)
            {
                protocol.Content = "";
            }
            else
            {
                protocol.Content = protocols[comboProj.SelectedValue.ToString()];
            }
            list = null;
            skipVial = null;
            ts = null;
            itemsToDo = null;
            lots = null;
            startIndexes = new List<int>() { 0, 0, 0, 0 };
        }
        private void setComboToBlank(int changeNo)
        {
            c4.SelectedIndex = -1;
            if (changeNo < 2)
            {
                c3.SelectedIndex = -1;
                if (changeNo < 1)
                {
                    c2.SelectedIndex = -1;
                    if (changeNo < 0)
                    {
                        c1.SelectedIndex = -1;
                    }
                }
            }
        }
        private void c1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setComboToBlank(0);
            var2.IsEnabled = false;
            var3.IsEnabled = false;
            var4.IsEnabled = false;
            if (c1.SelectedIndex != -1)
            {
                currentLCCondition = currentDataset[c1.SelectedIndex].LCCondition;
                if (currentDataset.FindAll(x => x.LCCondition == currentLCCondition).Count > 1)
                {
                    var2.IsEnabled = true;
                    c2.ItemsSource = currentDataset.Where(x =>
                    {
                        return x.LCCondition == currentLCCondition && itemParser[x.Name] != c1.SelectedItem.ToString();
                    }).Select(x => x.Name).Select(x => itemParser[x]);
                }
            }
        }
        private void c2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setComboToBlank(1);
            var3.IsEnabled = false;
            var4.IsEnabled = false;
            if (c2.SelectedIndex != -1)
            {
                if (currentDataset.FindAll(x => x.LCCondition == currentLCCondition).Count > 2)
                {
                    var3.IsEnabled = true;
                    c3.ItemsSource = currentDataset.Where(x =>
                    {
                        return x.LCCondition == currentLCCondition && itemParser[x.Name] != c2.SelectedItem.ToString() && itemParser[x.Name] != c1.SelectedItem.ToString();
                    }).Select(x => x.Name).Select(x => itemParser[x]);
                }
            }
        }
        private void c3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setComboToBlank(2);
            var4.IsEnabled = false;
            if (c3.SelectedIndex != -1)
            {
                if (currentDataset.FindAll(x => x.LCCondition == currentLCCondition).Count > 3)
                {
                    var4.IsEnabled = true;
                    c4.ItemsSource = currentDataset.Where(x =>
                    {
                        return x.LCCondition == currentLCCondition && itemParser[x.Name] != c3.SelectedItem.ToString() && itemParser[x.Name] != c2.SelectedItem.ToString() && itemParser[x.Name] != c1.SelectedItem.ToString();
                    }).Select(x => x.Name).Select(x => itemParser[x]);
                }
            }
        }
        private bool Validate()
        {
            return !(comboProj.SelectedIndex == -1 || c1.SelectedIndex == -1 || lot.Text == "");
        }
        private void copyA_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            Generate();
        }
        private void copyB_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            Generate();
        }
        private void copyC_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            Generate();
        }
        private void view_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            Generate();
        }
        // Var for generate function
        ////Vial, volume, #of inj, name, time
        List<ListEntry> list;                                               //最终序列表
        List<int> skipVial;                                                 //跳过瓶号列表
        public List<TaskSet> ts;                                            //批号对应的项目执行与否
        public List<string> itemsToDo;                                      //所有要进行的项目
        List<string> lots;                                                  //批号列表
        List<int> startIndexes;                                             //每个项目开始的瓶号，0--继续编号（默认），1--以x1开始
        int currentAvailVial = 1;
        string changeValidator = "";
        bool requireDifferentStd = false;
        List<int> stdTypes;
        
        private void Generate()
        {
            //由品名、批号、项目生成字符串，用于判断是否改变，若改变，则全部重新生成ts, lots, itemsToDo
            if (changeValidator != comboProj.SelectedValue + lot.Text + c1.SelectedValue + c2.SelectedValue + c3.SelectedValue + c4.SelectedValue)
            {
                changeValidator = comboProj.SelectedValue + lot.Text + c1.SelectedValue + c2.SelectedValue + c3.SelectedValue + c4.SelectedValue;
                ts = new List<TaskSet>();
                lots = lot.Text.Split('\n', '\r').ToList();
                lots.RemoveAll(x => x == "");
                itemsToDo = new List<string>
            {
                c1.Text
            };
                if (c2.Text != "")
                {
                    itemsToDo.Add(c2.Text);
                    if (c3.Text != "")
                    {
                        itemsToDo.Add(c3.Text);
                        if (c4.Text != "")
                        {
                            itemsToDo.Add(c4.Text);
                        }
                    }
                }
                foreach (var lotNo in lots)
                {
                    if (!ts.Contains(new TaskSet(lotNo, 1), new TaskSetComparer()))
                    {
                        ts.Add(new TaskSet(lotNo, itemsToDo.Count));
                    }
                }
                ts.RemoveAll(x => !lots.Contains(x.Lot));
            }
            var skipList = skip.Text.Split(',').ToList();
            skipList.RemoveAll(x => x == "");
            skipVial = skipList.Select(x => int.Parse(x)).ToList();
            //确认是否需要不同的STD
            if (itemsToDo.Count == 2 && !itemsToDo.Contains("有关物质") || itemsToDo.Count > 2)
            {
                stdTypes = new List<int>();
                foreach (var i in itemsToDo)
                {
                    requireDifferentStd = true;
                    stdTypes.Add(currentDataset.Find(x => x.Name == MethodEditor.Trans(i)).StdType);
                }
            }
            //特殊情况1：PRT有关物质和含量（或耐酸力）同时进行时，为FLD1，FLD2，LMD，YSY，KB1。。。
            //特殊情况2：GL有关物质若与含量合编，TW为STD2的瓶号
            //特殊情况3：CDN溶出样品后跟STD1为新瓶子
            currentAvailVial = 1;
            Dictionary<int, int> currentSTD1Vial = new Dictionary<int, int>();


        }
        #region detailed modification
        private void s1_Click(object sender, RoutedEventArgs e)
        {
            if (!c1.IsEnabled || c1.SelectedIndex == -1) return;
            me = new MethodEditor(comboProj.SelectedValue.ToString(), currentDataset, c1.SelectedValue.ToString(), 0, startIndexes)
            {
                ParentWindow = this
            };
            IsEnabled = false;
            me.Show();
        }
        private void s2_Click(object sender, RoutedEventArgs e)
        {
            if (!c2.IsEnabled || c2.SelectedIndex == -1) return;
            me = new MethodEditor(comboProj.SelectedValue.ToString(), currentDataset, c2.SelectedValue.ToString(), 1, startIndexes)
            {
                ParentWindow = this
            };
            IsEnabled = false;
            me.Show();

        }
        private void s3_Click(object sender, RoutedEventArgs e)
        {
            if (!c3.IsEnabled || c3.SelectedIndex == -1) return;
            me = new MethodEditor(comboProj.SelectedValue.ToString(), currentDataset, c3.SelectedValue.ToString(), 2, startIndexes)
            {
                ParentWindow = this
            };
            IsEnabled = false;
            me.Show();

        }
        private void s4_Click(object sender, RoutedEventArgs e)
        {
            if (!c4.IsEnabled || c4.SelectedIndex == -1) return;
            me = new MethodEditor(comboProj.SelectedValue.ToString(), currentDataset, c4.SelectedValue.ToString(), 3, startIndexes)
            {
                ParentWindow = this
            };
            IsEnabled = false;
            me.Show();

        }
        private void lotSet_Click(object sender, RoutedEventArgs e)
        {
            if (lot.Text != "" && c1.SelectedIndex !=-1)
            {
                Generate();
                ie = new ItemEditor(itemsToDo, ts)
                {
                    ParentWindow = this
                };
                IsEnabled = false;
                ie.Show();
            }
        }
        #endregion
        private void skip_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (skip.Text == "") return;            
            int temp = 0;
            if (skip.Text.Length == 1 && int.TryParse(skip.Text, out temp)) return;
            if ((skip.Text[skip.Text.Length - 1] == ',' )|| int.TryParse(skip.Text[skip.Text.Length - 1].ToString(), out temp)) return;
            skip.Text = skip.Text.Substring(0, skip.Text.Length - 1);
            skip.SelectionStart = skip.Text.Length;

        }

        //private SubList GenListDissolution(List<string> lots, ref int startVial, Item currentItem, bool sampleNewVial, List<int> skipVial, string config = "")
        //{
        //    SubList sl = new SubList();
        //    sl.StdType = currentItem.StdType;
        //    for (int i = 0; i < currentItem.Injs.Count; i++)
        //    {
        //        if (!currentItem.Injs[i].Name.Contains("sp"))
        //        {
        //            while(skipVial!=)
        //            sl.Add(startVial, currentItem.Injs[i]);
        //            startVial++;
        //        }
        //    }
            
            
        //}
    }   
}