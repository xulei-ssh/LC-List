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
    /// Auth.xaml 的交互逻辑
    /// </summary>
    public partial class Auth : Window
    {
        public new MainWindow Parent { get; set; }
        public MethodEditor ME { get; set; }
        public Config CO { get; set; }
        private bool displayMain;
        public List<UserInfo> users { get; set; }
        public int Type { get; set; }
        public Auth(MainWindow parent,int type)
        {
            Parent = parent;
            InitializeComponent();
            tName.Focus();
            displayMain = true;
            users = ConfigParser.ParseUser();
            Type = type;
        }
        
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (displayMain)
            {
                Parent.IsEnabled = true;
                Parent.Show();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Verify();

        }
        private void tConfirm(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Verify();
            }
        }
        private void Verify()
        {
            bool verified = false;
            UserInfo u = new UserInfo();
            if (tName.Text == "root")
            {
                List<string> passes = new List<string>();
                for (int i = -2; i < 3; i++)
                {
                    passes.Add(DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Hour.ToString() + (DateTime.Now.Minute + i).ToString());
                }
                if(passes.Contains (tPass.Password))
                {
                    u = users.Find(x => x.Name == "root");
                    verified = true;
                }
            }
            else
            {
                if (users.FindAll(x => x.Name == tName.Text).Count != 0)
                {
                    u = users.Find(x => x.Name == tName.Text);
                    if (u.Token == "")
                    {
                        IsEnabled = false;
                        NewUser nu = new NewUser(this);
                        if (nu.ShowDialog() == true)
                        {
                            verified = true;
                            nu.Close();
                        }
                        else
                        {
                            nu.Close();
                            return;
                        }
                    }
                    else 
                    {
                        if (SafeHandler.Hash(tPass.Password) == u.Token)
                        {
                            verified = true;
                        }
                    }
                }
            }
            if (u.Status == UserStatus.disabled)
            {
                tName.KeyUp -= tConfirm;
                tPass.KeyUp -= tConfirm;
                MessageBox.Show("User account disabled.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (verified)
            {
                if (Type == 1)
                {
                    ME = new MethodEditor(Parent, u);
                    ME.Show();
                    ME.Focus();
                }
                else
                {
                    CO = new Config(Parent, u);
                    CO.Show();
                    CO.Focus();
                }
                displayMain = false;
                Close();
            }
            else
            {
                tName.KeyUp -= tConfirm;
                tPass.KeyUp -= tConfirm;
                MessageBox.Show("Invalid username and/or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
