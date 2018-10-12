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
        public MethodEditor(MainWindow parent, UserInfo ui)
        {
            Parent = parent;
            InitializeComponent();
            database = ConfigParser.ParseDrug();
            cProj.Items.Clear();
            database.Keys.ToList().ForEach(x => cProj.Items.Add(x));
            if (ui.Group == UserGroup.analyst)
            {
                btnAddItem.IsEnabled = false;
                btnAddProj.IsEnabled = false;
                btnDelItem.IsEnabled = false;
                btnDelProj.IsEnabled = false;
                btnSave.IsEnabled = false;
                g.IsReadOnly = true;
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
        }
        public void cProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cProj.SelectedIndex != -1)
            {
                string fullProt = database[cProj.SelectedValue.ToString()].Protocol;
                tProt.Text = fullProt.Split(' ')[0];
                tVer.Text = fullProt.Split(' ')[1].Split('.')[1];
                cItem.SelectedIndex = -1;
                tSTD.Text = "";
                tCondition.Text = "";
                cConfig.Text = "";
                cItem.ItemsSource = database[cProj.SelectedValue.ToString()].Items.ConvertAll(x => x.Name);
                g.ItemsSource = null;
            }
            else
            {
                tProt.Text = "";
                tVer.Text = "";

                cItem.SelectedIndex = -1;
                tSTD.Text = "";
                tCondition.Text = "";
                cConfig.Text = "";
                cItem.ItemsSource = null;
                g.ItemsSource = null;
            }
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
            ConfigParser.SaveDrug(database);
        }
        private void g_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back) && ((DataGrid)sender).SelectedIndex != -1)
            {
                database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].DeleteInj(((DataGrid)sender).SelectedIndex);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            Parent.Show();
        }
        private void btnAddProj_Click(object sender, RoutedEventArgs e)
        {
            AddProj addProj = new AddProj(this, database);
            IsEnabled = false;
            addProj.Show();
        }
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cProj.SelectedIndex != -1)
            {
                AddItem addItem = new AddItem(this, database[cProj.SelectedValue.ToString()], cProj.SelectedValue.ToString());
                IsEnabled = false;
                addItem.Show();
            }
        }
        private void btnDelProj_Click(object sender, RoutedEventArgs e)
        {
            if (cProj.SelectedIndex != -1)
            {
                if (MessageBox.Show("Are you sure to delete Project: " + cProj.SelectedValue.ToString() + " ?\nThis action cannot be undone.", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string currentIndex = cProj.SelectedValue.ToString();
                    cProj.SelectedIndex = -1;
                    cProj.Items.Clear();
                    database.Remove(currentIndex);
                    database.Keys.ToList().ForEach(x => cProj.Items.Add(x));

                }
            }
        }
        private void btnDelItem_Click(object sender, RoutedEventArgs e)
        {
            if(cProj.SelectedIndex!=-1 && cItem.SelectedIndex!=-1)
            {
                if (MessageBox.Show("Are you sure to delete Item: " + cProj.SelectedValue.ToString() + " - " + cItem.SelectedValue.ToString() + " ?\nThis action cannot be undone.", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    database[cProj.SelectedValue.ToString()].Items.RemoveAll(x => x.Name == cItem.SelectedValue.ToString());
                    cItem.SelectedIndex = -1;
                    cItem.ItemsSource = database[cProj.SelectedValue.ToString()].Items.ConvertAll(x => x.Name);
                    g.ItemsSource = null;
                    tCondition.Text = "";
                    tSTD.Text = "";
                    cConfig.SelectedIndex = -1;
                }

            }
        }
    }
}
