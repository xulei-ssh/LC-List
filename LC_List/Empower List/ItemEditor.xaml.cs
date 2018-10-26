using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Empower_List
{

    public partial class ItemEditor : Window
    {
        public ProjSelect ParentWindow { get; set; }
        private List<TaskSet> tasks;
        public ItemEditor(ProjSelect parent,string[] headers, List<TaskSet> ts)
        {
            InitializeComponent();
            ParentWindow = parent;
            Col1.Visibility = Visibility.Hidden;
            Col2.Visibility = Visibility.Hidden;
            Col3.Visibility = Visibility.Hidden;
            Col4.Visibility = Visibility.Hidden;
            if (headers.Count() > 0)
            {
                Col1.Visibility = Visibility.Visible;
                Col1.Header = headers[0];
                if (headers.Count() > 1)
                {
                    Col2.Visibility = Visibility.Visible;
                    Col2.Header = headers[1];
                    if (headers.Count() > 2)
                    {
                        Col3.Visibility = Visibility.Visible;
                        Col3.Header = headers[2];
                        if (headers.Count() > 3)
                        {
                            Col4.Visibility = Visibility.Visible;
                            Col4.Header = headers[3];
                        }
                    }
                }
            }
            tasks = ts;
            grid.ItemsSource = ts;
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
            grid.CommitEdit(DataGridEditingUnit.Cell, true);
            grid.Items.Refresh();

        }
        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader != null)
            {
                int c = columnHeader.DisplayIndex;
                //get all properties
                bool allTrue = true;
                foreach (var q in tasks)
                {
                    if (!q.Items[c-1]) allTrue = false;
                }
                for (int i = 0; i < tasks.Count; i++)
                {
                    tasks[i].Items[c-1] = !allTrue;
                }
            }
            grid.ItemsSource = tasks;
            grid_Unloaded(grid, null);

        }
    }
}
