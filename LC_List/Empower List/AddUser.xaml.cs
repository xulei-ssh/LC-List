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
    public partial class AddUser : Window
    {
        Config Parent { get; set; }
        List<UserInfo> Users { get; set; }
        public AddUser(Config parent,List<UserInfo> users)
        {
            Parent = parent;
            Users = users;
            InitializeComponent();
            tName.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tName.Text != "" && !Users.Exists(x => x.Name == tName.Text))
            {
                Users.Add(new UserInfo(tName.Text, UserGroup.analyst, "", 1, UserStatus.enabled));
                Parent.userListGrid.Items.Refresh();
                Parent.IsEnabled = true;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Parent.IsEnabled = true;
            Close();
        }
    }
}
