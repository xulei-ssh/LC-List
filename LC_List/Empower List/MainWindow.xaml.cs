using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.IO;
using System.Windows;
namespace Empower_List
{
    public partial class MainWindow : System.Windows.Window
    {
        public Brush defaultBrush;
        public MainWindow()
        {
            InitializeComponent();
            defaultBrush = btnMethod.Foreground;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            lblVer.Content = "ver " + version.Major + "." + version.Minor + "." + version.Revision;
        }
        protected override void OnClosed(EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void btnMethod_MouseMove(object sender, MouseEventArgs e)
        {
            btnMethod.Foreground = new SolidColorBrush(Colors.Turquoise);
        }
        private void btnMethod_MouseLeave(object sender, MouseEventArgs e) => btnMethod.Foreground = defaultBrush;
        private void btnList_MouseMove(object sender, MouseEventArgs e) => btnList.Foreground = new SolidColorBrush(Colors.Turquoise);
        private void btnList_MouseLeave(object sender, MouseEventArgs e) => btnList.Foreground = defaultBrush;
        private void btnList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckFile())
            {
                ProjSelect a = new ProjSelect(this);
                Hide();
                a.Show();
                a.Focus();
            }
        }
        private void btnAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            About a = new About(this);
            this.IsEnabled = false;
            a.Show();
            a.Focus();
        }
        private void aboutMark_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnAbout_MouseUp(this, null);
        }
        private void btnMethod_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckFile())
            {
                Auth a = new Auth(this);
                IsEnabled = false;
                a.Show();
                Hide();
            }
        }
        private bool CheckFile()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ds"))
            {
                MessageBox.Show("Cannot find database file.", "Data File Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;

        }
    }
}
