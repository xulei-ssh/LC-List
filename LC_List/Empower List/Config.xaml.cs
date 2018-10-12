using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace Empower_List
{
    public partial class Config : Window
    {
        new MainWindow Parent { get; set; }
        UserInfo UI { get; set; }
        public List<bool> config { get; set; }
        List<UserInfo> users;
        public Config(MainWindow parent,UserInfo ui)
        {
            Parent = parent;
            UI = ui;
            InitializeComponent();
            users= ConfigParser.ParseUser();
            userListGrid.ItemsSource = users;
            if (UI.Group != UserGroup.root)
            {
                userListGrid.IsReadOnly = true;
                S1.IsEnabled = false;
                S2.IsEnabled = false;
                S3.IsEnabled = false;
            }
            config = ConfigParser.ParseConfig();
            S1.IsChecked = config[0];
            S2.IsChecked = config[1];
            S3.IsChecked = config[2];
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
        }
        private void userListGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if ( e.Key == Key.Back && ((DataGrid)sender).SelectedIndex > 0)
            {
                users.RemoveAt(((DataGrid)sender).SelectedIndex);
                userListGrid.Items.Refresh();
            }
        }
        private void btnSaveA_Click(object sender, RoutedEventArgs e) => ConfigParser.SaveUser(users);
        private void btnSaveB_Click(object sender, RoutedEventArgs e) => ConfigParser.SaveConfig(config);
        private void S1_Checked(object sender, RoutedEventArgs e) => config[0] = true;
        private void S1_Unchecked(object sender, RoutedEventArgs e) => config[0] = false;
        private void S2_Checked(object sender, RoutedEventArgs e) => config[1] = true;
        private void S2_Unchecked(object sender, RoutedEventArgs e) => config[1] = false;
        private void S3_Checked(object sender, RoutedEventArgs e) => config[2] = true;
        private void S3_Unchecked(object sender, RoutedEventArgs e) => config[2] = false;


    }

}
