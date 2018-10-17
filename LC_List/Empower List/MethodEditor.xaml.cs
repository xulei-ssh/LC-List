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
        UserInfo UI { get; set; }
        public MethodEditor(MainWindow parent, UserInfo ui)
        {
            Parent = parent;
            UI = ui;
            InitializeComponent();
            database = ConfigParser.ParseDrug();
            cProj.Items.Clear();
            database.Keys.ToList().ForEach(x => cProj.Items.Add(x));
            SetAcccess();
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
                cItem.ItemsSource = null;
                g.ItemsSource = null;
            }
            SetAcccess();
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
                }
                else
                {
                    tSTD.IsEnabled = true;
                    tSTD.Text = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].StdType.ToString();
                }
                g.ItemsSource = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].Injs;
            }
            SetAcccess();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyList())
            {
                ConfigParser.SaveDrug(database);
                MessageBox.Show("Method Saved.", "Success", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
            }
            else
            {
                AlertListError();
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
            if (cProj.SelectedIndex != -1 && cItem.SelectedIndex != -1)
            {
                if (MessageBox.Show("Are you sure to delete Item: " + cProj.SelectedValue.ToString() + " - " + cItem.SelectedValue.ToString() + " ?\nThis action cannot be undone.", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    database[cProj.SelectedValue.ToString()].Items.RemoveAll(x => x.Name == cItem.SelectedValue.ToString());
                    cItem.SelectedIndex = -1;
                    cItem.ItemsSource = database[cProj.SelectedValue.ToString()].Items.ConvertAll(x => x.Name);
                    g.ItemsSource = null;
                    tCondition.Text = "";
                    tSTD.Text = "";
                }

            }
        }
        private void Prot_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cProj.SelectedIndex != -1)
            {
                if (tProt.Text.Contains(".") || tVer.Text.Contains("."))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(0, ((TextBox)sender).Text.Length - 1);
                    return;
                }
                database[cProj.SelectedValue.ToString()].Protocol = tProt.Text + " ver." + tVer.Text;
            }
        }
        private void tCondition_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cProj.SelectedIndex != -1 && cItem.SelectedIndex != -1)
            {
                if (tCondition.Text != "" && !int.TryParse(tCondition.Text, out int temp))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(0, ((TextBox)sender).Text.Length - 1);
                    return;
                }
                database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].LCCondition = tCondition.Text == "" ? 0 : int.Parse(tCondition.Text);
            }
        }
        private void tSTD_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cProj.SelectedIndex != -1 && cItem.SelectedIndex != -1)
            {
                if (tSTD.Text != "" && !int.TryParse(tSTD.Text, out int temp))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(0, ((TextBox)sender).Text.Length - 1);
                    return;
                }
                database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].StdType = tSTD.Text == "" ? 0 : int.Parse(tSTD.Text);
            }
        }
        public bool VerifyList()
        {
            if (cProj.SelectedIndex == -1 || cItem.SelectedIndex == -1) return true;
            var t = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()];
            if (((t.Name == "Related Substance") && (t.Injs.Count > 1) && (t.Injs.Last().Name == "spY")) || ((t.Name != "Related Substance") && (t.Injs.Count > 1) && (t.Injs.Last().Name == "sp")))
            {
                return true;
            }
            return false;
        }
        private void g_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!VerifyList())
            {
                AlertListError();
            }
        }
        public void AlertListError()
        {
            MessageBox.Show("List must have a sample entry named \"spY\" (for Related Substance) or \"sp\" (for other Items).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void SetAcccess()
        {
            if (UI.Group == UserGroup.analyst)
            {
                btnAddItem.IsEnabled = false;
                btnAddProj.IsEnabled = false;
                btnDelItem.IsEnabled = false;
                btnDelProj.IsEnabled = false;
                btnSave.IsEnabled = false;
                g.IsReadOnly = true;
                tProt.IsEnabled = false;
                tVer.IsEnabled = false;
                tCondition.IsEnabled = false;
                tSTD.IsEnabled = false;
            }
        }
        private void g_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (g.SelectedIndex != -1)
            {
                database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].Injs.RemoveAt(g.SelectedIndex);

            }
        }
    }
}
