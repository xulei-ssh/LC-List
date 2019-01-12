using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.IO;
using System.Windows;
namespace Empower_List
{
    public partial class MainWindow : Window
    {
        public Brush defaultBrush;
        public MainWindow()
        {
            InitializeComponent();
            defaultBrush = btnMethod.Foreground;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            lblVer.Content = "ver " + version.Major + "." + version.Minor + "." + version.Revision;
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"notification"))
                {
                    var notification = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"notification");
                    MessageBox.Show(notification.ReadLine(), notification.ReadLine());
                }
            }
            catch (Exception e) { }
        }
        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMethod_MouseMove(object sender, MouseEventArgs e) => btnMethod.Foreground = new SolidColorBrush(Colors.Turquoise);
        private void btnMethod_MouseLeave(object sender, MouseEventArgs e) => btnMethod.Foreground = defaultBrush;
        private void btnList_MouseMove(object sender, MouseEventArgs e) => btnList.Foreground = new SolidColorBrush(Colors.Turquoise);
        private void btnList_MouseLeave(object sender, MouseEventArgs e) => btnList.Foreground = defaultBrush;
        private void btnConfig_MouseMove(object sender, MouseEventArgs e) => btnConfig.Foreground = new SolidColorBrush(Colors.Turquoise);
        private void btnConfig_MouseLeave(object sender, MouseEventArgs e) => btnConfig.Foreground = defaultBrush;
        private void btnWord_MouseMove(object sender, MouseEventArgs e) => btnWord.Foreground = new SolidColorBrush(Colors.Turquoise);
        private void btnWord_MouseLeave(object sender, MouseEventArgs e) => btnWord.Foreground = defaultBrush;
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
            IsEnabled = false;
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
                Auth a = new Auth(this,1);
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
        private void btnConfig_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckFile())
            {
                Auth a = new Auth(this, 2);
                IsEnabled = false;
                a.Show();
                Hide();
            }

        }
        private void btnWord_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Sequences"))
            {
                MessageBox.Show("Cannot find list directory.", "List Directory Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Sequences").Length==0)
            {
                MessageBox.Show("Cannot find list file.", "List File Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Gen g = new Gen(this);
            IsEnabled = false;
            g.Show();
            Hide();
        }
        private void btnFAQ_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string loc = AppDomain.CurrentDomain.BaseDirectory + @"UserGuide.docx";
            if (File.Exists(loc))
            {
                System.Diagnostics.Process.Start("winword.exe", loc);
            }
            else
            {
                MessageBox.Show("Cannot locate help file.", "File missing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FAQMark_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnFAQ_MouseUp(this, null);
        }
    }
}
