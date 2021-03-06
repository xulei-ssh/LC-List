﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
namespace Empower_List
{
    public partial class ProjSelect : Window
    {
        Dictionary<string, ProjectInfo> database;
        List<TaskSet> tasks;
        bool openedDetails = false;
        new MainWindow Parent { get; set; }
        List<bool> config;
        PlateStyle plateStyle;
        //Dictionary<string, string[]> ItemEachLot;
        public ProjSelect(MainWindow parent)
        {
            InitializeComponent();
            //Set lot hint
            lotHint.Content = "稳定性简便输入格式：\n批号+空格+条件(A/L/T)+月份\n例如：\n\"111AK L3\"相当于111AK(25*60,3M)\n\"111AK A6\"相当于111AK(40*75,6M)\n\"111AK T44\"相当于111AK(30*65,44M)";
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = "序列表编辑器 " + version.Major + "." + version.Minor + "." + version.Revision;
            Parent = parent;
            if(!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"ds"))
            {
                MessageBox.Show("找不到数据库","错误");
                Hide();
                return;
            }
            database = ConfigParser.ParseDrug();
            config = ConfigParser.ParseConfig();
            plateStyle100.IsChecked = true;
            plateStyle = PlateStyle.Normal;
            lblRT.IsEnabled = config[0];
            uTime.IsEnabled = config[0];
            SetTime.IsEnabled = config[0];
            methodGrid.IsReadOnly = !config[0];
            comboProj.Items.Clear();
            database.Keys.ToList().ForEach(x => comboProj.Items.Add(x));
            tip.Text = "1: 请务必确认SOP版本；当前版本仅支持单张序列表\n2: 稳定性样品请用小括号注明贮存条件，或使用简便格式\n3: CDN溶出度不得与其他项使用同一个序列表";
            #region delete after 2 list done

            #endregion
        }
        private void comboProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblProtocol.Content = database[comboProj.SelectedValue.ToString()].Protocol;
            comboItems.Items.Clear();
            listItems.Items.Clear();
            comboItems.Items.Add("<< Clear All >>");
            database[comboProj.SelectedValue.ToString()].Items.ToList().ForEach(x => comboItems.Items.Add(x.Name));
            ModifySwitch(true);
            tasks = null;
            openedDetails = false;
            cNewVial.IsEnabled = true;
            cNewVial.IsChecked = false;

        }
        private void comboItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboItems.SelectedIndex == -1) return;
            if (comboItems.SelectedValue.ToString() == "<< Clear All >>")
            {
                listItems.Items.Clear();
            }
            else if (listItems.Items.Count == 0)
            {
                listItems.Items.Add(database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].Name);
                database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].NewLine = false;
            }
            else
            {
                if (!listItems.Items.Contains(database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].Name) && (database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].LCCondition == database[comboProj.SelectedValue.ToString()][listItems.Items[listItems.Items.Count - 1].ToString()].LCCondition))
                {
                    listItems.Items.Add(database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].Name);
                    database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].NewLine = false;
                }
            }
            UpdateItemEachLot();
            if (listItems.HasItems)
            {
                listItems.SelectedIndex = listItems.Items.Count - 1;
            }

        }
        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listItems.Items != null && listItems.SelectedIndex != -1)
            {
                preview.Header = comboProj.SelectedValue.ToString() + " -- " + listItems.SelectedValue.ToString();
                methodGrid.ItemsSource = database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].Injs;
                radioNew.IsChecked = database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewLine;
                radioNormal.IsChecked = !database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewLine;
                uTime.Text = "";
                if (listItems.SelectedValue.ToString() == "Related Substance" || listItems.SelectedValue.ToString() == "Assay")
                {
                    radioNew.Content = "项目从下一个空列开始";
                }
                else
                {
                    radioNew.Content = "样品从下一个空列开始";
                    
                }
                if (listItems.SelectedValue.ToString() == "Related Substance")
                {
                    cNewVial.IsChecked = false;
                    cNewVial.IsEnabled = false;
                }
                else
                {
                    cNewVial.IsEnabled = true;
                    cNewVial.IsChecked = database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewStd;
                }
                
            }
            else
            {
                preview.Header = "方法预览";
                uTime.Text = "";
                methodGrid.ItemsSource = null;
            }
        }
        private void SetTime_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(uTime.Text, out double temp))
            {
                MessageBox.Show("运行时间格式错误", "时间无效", MessageBoxButton.OK, MessageBoxImage.Error);
                uTime.SelectAll();
                uTime.Focus();
            }
            else
            {
                if (methodGrid.Items.Count != 0)
                {
                    double time = double.Parse(uTime.Text);
                    foreach (Inj item in database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].Injs)
                    {
                        item.Time = time;
                    }
                    methodGrid.ItemsSource = database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].Injs;
                }
            }
        }
        private void radioNormal_Click(object sender, RoutedEventArgs e)
        {
            if (listItems.Items != null && listItems.SelectedIndex != -1)
            {
                database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewLine = (bool)radioNew.IsChecked;
            }
        }
        private void radioNew_Click(object sender, RoutedEventArgs e)
        {
            radioNormal_Click(this, null);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.Show();
        }
        private void btnLotDetail_Click(object sender, RoutedEventArgs e)
        {
            if (tasks == null || textLots.Text == "" || listItems.Items == null || listItems.Items.Count == 0) return;
            if (openedDetails == false)
            {
                UpdateItemEachLot();
                openedDetails = true;
            }
            ModifySwitch(false);
            string[] headers = new string[listItems.Items.Count];
            listItems.Items.CopyTo(headers, 0);
            ItemEditor ie = new ItemEditor(this, headers, tasks);
            ie.Show();
            IsEnabled = false;
        } 
        private void ModifySwitch(bool mode)
        {
            textLots.IsEnabled = mode;
            comboItems.IsEnabled = mode;
            listItems.IsEnabled = mode;
        }
        private void UpdateItemEachLot()
        {
            Regex reg = new Regex(@" ([L|A|T])([0-9]{1,2})(\b)");
            tasks = new List<TaskSet>();
            foreach (var lot in textLots.Text.Split('\r','\n'))
            {
                if (lot != "")
                {
                    //
                    Match match = reg.Match(lot);
                    if (match.Success)
                    {
                        tasks.Add(new TaskSet(lot.Replace(" A", @"(40*75,")
                            .Replace(" L", @"(25*60,")
                            .Replace(" T", @"(30*65,") + "M)"
                            , listItems.Items.Count));

                    }
                    else
                    {
                        tasks.Add(new TaskSet(lot, listItems.Items.Count));
                    }
                    //

                }
            }          
        }
        private void textLots_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateItemEachLot();
        }
        private void listItems_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateItemEachLot();
        }
        private void btnGen_Click(object sender, RoutedEventArgs e)
        {
            if (tasks == null || textLots.Text == "" || listItems.Items == null || listItems.Items.Count == 0) return;

            // Check skip Vial 
            if (!SkipVerify())
            {
                MessageBox.Show("跳过瓶号必须为数字或1:A,1型", "跳过瓶号无效", MessageBoxButton.OK, MessageBoxImage.Error);
                textSkip.SelectAll();
                textSkip.Focus();
                return;
            }
            if (!FirstVerify())
            {
                MessageBox.Show("开始瓶号必须为数字或1:A,1型", "开始瓶号无效", MessageBoxButton.OK, MessageBoxImage.Error);
                textFirstVial.SelectAll();
                textFirstVial.Focus();
                return;
            }

            if (openedDetails == false)
            {
                UpdateItemEachLot();
                openedDetails = true;
            }
            ModifySwitch(false);
            string[] headers = new string[listItems.Items.Count];
            listItems.Items.CopyTo(headers, 0);
            var lots = textLots.Text.Split('\r', '\n');

            FinalList fl = new FinalList(this, comboProj.SelectedValue.ToString(), database[comboProj.SelectedValue.ToString()], headers, tasks, plateStyle, textSkip.Text.Trim(), lots, textFirstVial.Text.Trim());
            fl.listName.IsReadOnly = !config[1];
            fl.listTime.IsReadOnly = !config[2];
            fl.Show();
            IsEnabled = false;
        }
        private void uTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetTime_Click(this, null);
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (comboProj.SelectedIndex != -1) comboProj_SelectionChanged(null, null);
        }
        private void textLots_KeyUp(object sender, KeyEventArgs e)
        {
            lotHint.Visibility = textLots.Text == "" ? Visibility.Visible : Visibility.Hidden;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                cLot.IsChecked = !cLot.IsChecked;
            }
            if (e.Key == Key.Enter && (bool)cLot.IsChecked)
            {
                if (textLots.SelectionStart != textLots.Text.Length)
                {
                    return;
                }
                //Get last line of previous
                string x = textLots.GetLineText(textLots.GetLastVisibleLineIndex() - 1);



                //假定开始都是数字，获取第一个不是数字的部分,将之后的保存
                string suffix = "", prefix = "";
                bool prefixDone = false;
                int startIndex = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    if (!int.TryParse(x[i].ToString(), out int q))
                    {
                        if (!prefixDone)
                        {
                            prefix += x[i];
                        }
                        else
                        {
                            suffix = x.Substring(i, x.Length - i);
                            x = x.Substring(startIndex, i - startIndex);
                            break;
                        }
                    }
                    else
                    {
                        if (!prefixDone) startIndex = i;
                        if (i == x.Length - 1)
                        {
                            x = x.Substring(startIndex, i - startIndex + 1);
                            break;
                        }
                        prefixDone = true;
                    }
                }
                if (x.Trim() == "") return;
                x = (long.Parse(x) + 1).ToString("D" + x.Length.ToString());
                x = prefix + x + suffix;
                x = x.Substring(0, x.Length - 2);
                textLots.Text += x;
                textLots.Select(textLots.Text.Length, 0);
                textLots.ScrollToEnd();
            }
        }
        private void cEleven_Click(object sender, RoutedEventArgs e)
        {

            //if (cEleven.IsChecked == true)
            //{
            //    cMax.SelectedIndex = 2;
            //    cMax.IsEnabled = false;
            //    textSkip.IsEnabled = false;
            //}
            //else
            //{
            //    cMax.SelectedIndex = 0;
            //    cMax.IsEnabled = true;
            //    textSkip.IsEnabled = true;
            //}
        }
        private bool SkipVerify()
        {
            if (textSkip.Text.Trim() == "") return true;
            //数字瓶号
            if (plateStyle == PlateStyle.Normal)
            {
                if (!int.TryParse(textSkip.Text.Trim(), out int temp))
                {
                    return false;
                }
            }
            // 字母瓶号
            else
            {
                string s = textSkip.Text.Trim();
                //5位以下，无效
                if (s.Length < 5) return false;
                if (s[1] != ':' || s[3] != ',') return false;
                var splits = s.Split(':', ',');
                if (splits.Count() != 3) return false;
                splits[1] = splits[1].ToUpper();
                if (splits[0] != "1" && splits[0] != "2") return false;
                if (splits[1] != "A" && splits[1] != "B" && splits[1] != "C" && splits[1] != "D" && splits[1] != "E" && splits[1] != "F") return false;
                if (!int.TryParse(splits[2], out int c)) return false;
                if (c < 1 || (plateStyle == PlateStyle.NewA && c > 11) || (plateStyle == PlateStyle.NewB && c > 8)) return false;
            }
            return true;
        }
        private bool FirstVerify()
        {
            if (textFirstVial.Text.Trim() == "") return true;
            //数字瓶号
            if (plateStyle == PlateStyle.Normal)
            {
                if (!int.TryParse(textFirstVial.Text.Trim(), out int temp))
                {
                    return false;
                }
            }
            // 字母瓶号
            else
            {
                string s = textFirstVial.Text.Trim();
                //5位以下，无效
                if (s.Length < 5) return false;
                if (s[1] != ':' || s[3] != ',') return false;
                var splits = s.Split(':', ',');
                if (splits.Count() != 3) return false;
                splits[1] = splits[1].ToUpper();
                if (splits[0] != "1" && splits[0] != "2") return false;
                if (splits[1] != "A" && splits[1] != "B" && splits[1] != "C" && splits[1] != "D" && splits[1] != "E" && splits[1] != "F") return false;
                if (!int.TryParse(splits[2], out int c)) return false;
                if (c < 1 || (plateStyle == PlateStyle.NewA && c > 11) || (plateStyle == PlateStyle.NewB && c > 8)) return false;
            }
            return true;

        }

        private void cNewVial_Checked(object sender, RoutedEventArgs e)
        {
            if (comboProj.SelectedIndex != -1 && listItems.SelectedIndex != -1 && cNewVial.IsEnabled)
            {
                database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewStd = true;
            }
        }

        private void cNewVial_Unchecked(object sender, RoutedEventArgs e)
        {
            if (comboProj.SelectedIndex != -1 && listItems.SelectedIndex != -1 && cNewVial.IsEnabled)
            {
                database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].NewStd = false;
            }

        }

        private void plateStyle100_Checked(object sender, RoutedEventArgs e) => plateStyle = PlateStyle.Normal;
        private void plateStyle132_Checked(object sender, RoutedEventArgs e) => plateStyle = PlateStyle.NewA;
        private void plateStyle96_Checked(object sender, RoutedEventArgs e) => plateStyle = PlateStyle.NewB;
    }
}
