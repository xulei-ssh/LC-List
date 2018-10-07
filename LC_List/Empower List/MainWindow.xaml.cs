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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using Table = Microsoft.Office.Interop.Word.Table;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;

namespace Empower_List
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public Brush defaultBrush;
        // Var for initialization
        ProjSelect p;
        Auth a;
        MethodEditor me;
        public MainWindow()
        {
            InitializeComponent();
            defaultBrush = btnMethod.Foreground;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            lblVer.Content = "ver " + version.Major + "." + version.Minor + "." + version.Revision;
        }
        protected override void OnClosed(EventArgs e)
        {
            var collections = System.Windows.Application.Current.Windows;

            foreach (System.Windows.Window window in collections)
            {
                if (window != this)
                    window.Close();
            }

            base.OnClosed(e);
            
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
            if (p == null) p = new ProjSelect();
            p.Show();
            p.Focus();
        }

        private void btnAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            About a = new About(this);
            
            this.IsEnabled = false;
            a.Show();

        }

        private void aboutMark_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnAbout_MouseUp(this, null);
        }

        private void btnWord_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //CreateDoc c = new CreateDoc();
            //c.CreateNewDocument(@"d:\a.dotx");
        }

        private void btnMethod_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (a == null) a = new Auth(this, new MethodEditor());

            this.IsEnabled = false;
            a.Show();


        }
    }
}
