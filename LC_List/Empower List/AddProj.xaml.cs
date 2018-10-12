using System;
using System.Collections.Generic;
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
    /// AddProj.xaml 的交互逻辑
    /// </summary>
    public partial class AddProj : Window
    {
        new MethodEditor Parent { get; set; }
        Dictionary<string, ProjectInfo> Database { get; set; }
        public AddProj(MethodEditor me, Dictionary<string, ProjectInfo> database)
        {
            InitializeComponent();
            Parent = me;
            Database = database;
            projName.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Database.ContainsKey(projName.Text))
            {
                MessageBox.Show("Project already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                projVer.KeyUp -= projVer_KeyUp;
            }
            else
            {
                if(!int.TryParse (projVer.Text,out int temp))
                {
                    MessageBox.Show("Version should be number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    projVer.KeyUp -= projVer_KeyUp;
                }
                else
                {
                    if (MessageBox.Show("Add "+projName.Text +" (" +projProt.Text+", Version: "+projVer.Text +") as a new project?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                    {
                        Database.Add(projName.Text, new ProjectInfo(projProt.Text + " ver." + projVer.Text));
                        Parent.cProj.Items.Clear();
                        Database.Keys.ToList().ForEach(x => Parent.cProj.Items.Add(x));
                        Parent.cProj.SelectedIndex = Parent.cProj.Items.Count - 1;
                        Close();
                    }
                }
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void projVer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(null, null);
            }
        }

        private void projVer_GotFocus(object sender, RoutedEventArgs e)
        {
            projVer.KeyUp -= projVer_KeyUp;
            projVer.KeyUp += projVer_KeyUp;
        }
    }
}
