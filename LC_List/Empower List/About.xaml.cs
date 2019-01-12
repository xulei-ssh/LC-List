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
using System.Reflection;

namespace Empower_List
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        Window parent;
        public About(Window p)
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            ver.Content = "版本：" + version.Major + "." + version.Minor + "." + version.Revision;
            parent = p;
            t.Text = "This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.\n\n"+
                "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.\n\n" +
                "You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.";



        }

        private void about_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parent.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
