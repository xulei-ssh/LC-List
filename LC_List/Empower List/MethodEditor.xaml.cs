using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Empower_List
{
    public partial class MethodEditor : Window
    {
        Dictionary<string, ProjectInfo> database;
        new MainWindow Parent { get; set; }

        public MethodEditor(MainWindow parent)
        {
            Parent = parent;
            InitializeComponent();
            database = ConfigParser.Parse(AppDomain.CurrentDomain.BaseDirectory + @"ds");
            cProj.Items.Clear();
            database.Keys.ToList().ForEach(x => cProj.Items.Add(x));
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
        }
        private void cProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fullProt= database[cProj.SelectedValue.ToString()].Protocol;
            tProt.Text = fullProt.Split(' ')[0];
            tVer.Text = fullProt.Split(' ')[1].Split('.')[1];
            cItem.SelectedIndex = -1;
            tSTD.Text = "";
            tCondition.Text = "";
            cConfig.Text = "";
            cItem.ItemsSource = database[cProj.SelectedValue.ToString()].Items.ConvertAll(x => x.Name);
        }

        private void cItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cItem.SelectedIndex != -1)
            {
                tCondition.Text = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].LCCondition.ToString();
                if (cItem.SelectedValue.ToString() == "Related Substance")
                {
                    tSTD.IsEnabled = false;
                    tSTD.Text = "";
                    cConfig.IsEnabled = true;
                    cConfig.Items.Add("self");
                    cConfig.Items.Add("union");
                    cConfig.SelectedIndex = database[cProj.SelectedValue.ToString()]["Related Substance"].Config == "self" ? 0 : 1;
                }
                else
                {
                    tSTD.IsEnabled = true;
                    tSTD.Text = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].StdType.ToString();
                    cConfig.IsEnabled = false;
                    cConfig.Items.Clear();
                }
                g.ItemsSource = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].Injs;


            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void g_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back) && ((DataGrid)sender).SelectedIndex != -1)
            {
                database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].DeleteInj(((DataGrid)sender).SelectedIndex);
            }
        }

        //private void g_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    if (e.EditAction == DataGridEditAction.Commit)
        //    {
        //        Item i = new Item();
        //        i.LCCondition = int.Parse(tCondition.Text);
        //        i.StdType = int.TryParse(tSTD.Text, out int temp) ? int.Parse(tSTD.Text) : 0;
        //        i.Config = cConfig.SelectedIndex == -1 ? "" : cConfig.SelectedValue.ToString();
        //        foreach (Inj row in ((DataGrid)sender).Items)
        //        {
        //            if (row.Name != "")
        //            {
        //                i.AddInj(row);
        //            }
        //        }
        //        database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()] = i;
        //    }
        //}

        //private void g_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (((DataGrid)sender).SelectedIndex != -1 && (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back))
        //    {
        //        database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].DeleteInj(((DataGrid)sender).SelectedIndex);
        //    }
        //}
    }
}
