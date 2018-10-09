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
        private bool displayMain;
        public Auth(MainWindow parent)
        {
            Parent = parent;
            InitializeComponent();
            tName.Focus();
            displayMain = true;
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
            List<string> passes = new List<string>();
            for (int i = -2; i < 3; i++)
            {
                passes.Add(DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Hour.ToString() + (DateTime.Now.Minute + i).ToString());
            }
            if (tName.Text == "root" && passes.Contains(tPass.Password))
            {
                ME = new MethodEditor(Parent);
                ME.Show();
                ME.Focus();
                displayMain = false;
                Close();
            }
            else
            {
                tName.KeyUp -= tConfirm;
                tPass.KeyUp -= tConfirm;
                MessageBox.Show("Invalid username and/or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                //tName.KeyUp += tConfirm;
                //tPass.KeyUp += tConfirm;
            }

        }
    }
}
