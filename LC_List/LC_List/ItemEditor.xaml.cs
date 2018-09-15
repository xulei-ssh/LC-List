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

namespace LC_List
{
    /// <summary>
    /// ItemEditor.xaml 的交互逻辑
    /// </summary>
    
    public partial class ItemEditor : Window
    {
        public MainWindow ParentWindow { get; set; }
        public ItemEditor(List<string> headers, List<TaskSet> fs)
        {
            InitializeComponent();
            Col1.Visibility = Visibility.Hidden;
            Col2.Visibility = Visibility.Hidden;
            Col3.Visibility = Visibility.Hidden;
            Col4.Visibility = Visibility.Hidden;
            if (headers.Count > 0)
            {
                Col1.Visibility = Visibility.Visible;
                Col1.Header = headers[0];
                if (headers.Count > 1)
                {
                    Col2.Visibility = Visibility.Visible;
                    Col2.Header = headers[1];
                    if (headers.Count > 2)
                    {
                        Col3.Visibility = Visibility.Visible;
                        Col3.Header = headers[2];
                        if (headers.Count > 3)
                        {
                            Col4.Visibility = Visibility.Visible;
                            Col4.Header = headers[3];
                        }
                    }
                }
            }
            grid.ItemsSource = fs;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.IsEnabled = true;
            ParentWindow.Show();
        }

        private void grid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }
    }
}
