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

namespace Empower_List
{
    /// <summary>
    /// Auth.xaml 的交互逻辑
    /// </summary>
    public partial class Auth : Window
    {
        public MainWindow Parent { get; set; }
        public MethodEditor ME { get; set; }
        public Auth(MainWindow parent,MethodEditor me)
        {
            InitializeComponent();
            Parent = parent;
            ME = me;
            tName.Focus();

        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
            this.Close();

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
            base.OnClosing(e);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            List<string> passes = new List<string>();
            for (int i = -2; i < 3; i++)
            {
                passes.Add(DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Hour.ToString() + (DateTime.Now.Minute + i).ToString());
            }
            if (tName.Text == "root" && passes.Contains(tPass.Password))
            {
                ME.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username and/or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            

        }

        private void tName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(this, null);
            }
        }

        private void tPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(this, null);
            }

        }
    }
}
