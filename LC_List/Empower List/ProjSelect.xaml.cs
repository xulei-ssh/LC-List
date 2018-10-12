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
using System.IO;
namespace Empower_List
{
    /// <summary>
    /// ProjSelect.xaml 的交互逻辑
    /// </summary>
    public partial class ProjSelect : Window
    {
        Dictionary<string, ProjectInfo> database;
        List<TaskSet> tasks;
        bool openedDetails = false;
        new MainWindow Parent { get; set; }
        List<bool> config;
        //Dictionary<string, string[]> ItemEachLot;
        public ProjSelect(MainWindow parent)
        {
            InitializeComponent();
            Parent = parent;
            if(!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"ds"))
            {
                MessageBox.Show("Cannot find database file.");
                Hide();
                return;
            }
            database = ConfigParser.ParseDrug();
            config = ConfigParser.ParseConfig();
            lblRT.IsEnabled = config[0];
            uTime.IsEnabled = config[0];
            SetTime.IsEnabled = config[0];
            methodGrid.IsReadOnly = config[0];
            comboProj.Items.Clear();
            database.Keys.ToList().ForEach(x => comboProj.Items.Add(x));
            tip.Text = "1: Item priority in sequence: Related Substance > Assay > Others.\n2: Use braces like (25*60, 18M) in Lot to indicate stability batches.\n3: CDN Dissolution cannot be carried out using a multi-item list.";
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
            }
            else
            {
                if (!listItems.Items.Contains(database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].Name) && (database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].LCCondition == database[comboProj.SelectedValue.ToString()][listItems.Items[listItems.Items.Count - 1].ToString()].LCCondition))
                {
                    listItems.Items.Add(database[comboProj.SelectedValue.ToString()][comboItems.SelectedValue.ToString()].Name);
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
                    radioNew.IsEnabled = false;
                    radioNormal.IsEnabled = false;
                    radioNormal.IsChecked = true;
                }
                else
                {
                    radioNew.IsEnabled = true;
                    radioNormal.IsEnabled = true;
                }
            }
            else
            {
                preview.Header = "Method Preview";
                uTime.Text = "";
                methodGrid.ItemsSource = null;
            }
        }
        private void SetTime_Click(object sender, RoutedEventArgs e)
        {
            double temp;
            if (!double.TryParse(uTime.Text, out temp))
            {
                MessageBox.Show("Invalid time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                }
                methodGrid.ItemsSource = database[comboProj.SelectedValue.ToString()][listItems.SelectedValue.ToString()].Injs;
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
            if (MessageBox.Show("Please confirm Lot, Item and Method information before customizing.\nNo further change could be made if continue.", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                if (openedDetails == false)
                {
                    UpdateItemEachLot();
                    openedDetails = true;
                }
                ModifySwitch(false);
                string[] headers = new string[listItems.Items.Count];
                listItems.Items.CopyTo(headers, 0);
                ItemEditor ie = new ItemEditor(this,headers, tasks);
                ie.Show();
                IsEnabled = false;

                
            }
        } 
        private void ModifySwitch(bool mode)     
        {
            textLots.IsEnabled = mode;
            comboItems.IsEnabled = mode;
            listItems.IsEnabled = mode;
            textSkip.IsEnabled = mode;
            preview.IsEnabled = mode;
        }
        private void UpdateItemEachLot()
        {
            tasks = new List<TaskSet>();
            foreach (var lot in textLots.Text.Split('\r','\n'))
            {
                if (lot != "")
                {
                    tasks.Add(new TaskSet(lot, listItems.Items.Count));
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
            int temp;
            if (textSkip.Text != "" && !int.TryParse(textSkip.Text, out temp))
            {
                MessageBox.Show("Invalid skipped vial", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                textSkip.SelectAll();
                textSkip.Focus();
                return;
            }

            if (MessageBox.Show("Please confirm Lot, Item and Method information before generating.\nNo further change could be made if continue.", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                if (openedDetails == false)
                {
                    UpdateItemEachLot();
                    openedDetails = true;
                }
                ModifySwitch(false);
                string[] headers = new string[listItems.Items.Count];
                listItems.Items.CopyTo(headers, 0);
                FinalList fl = new FinalList(this,comboProj.SelectedValue.ToString(), database[comboProj.SelectedValue.ToString()], headers, tasks, textSkip.Text == "" ? 0 : int.Parse(textSkip.Text));
                fl.listName.IsReadOnly = !config[1];
                fl.listTime.IsReadOnly = !config[2];
                fl.Show();

                this.IsEnabled = false;


            }

        }
        private void uTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetTime_Click(this, null);
            }
        }
    }
}
