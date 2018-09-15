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
    /// MethodEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MethodEditor : Window
    {
        public MainWindow ParentWindow { get; set; }
        int CIndex { get; set; }
        List<int> StartVial { get; set; }
        public MethodEditor(string projName, List<Item> currentDataset, string selectedValue, int cIndex, List<int> startVial)
        {
            InitializeComponent();
            Item currentItem = currentDataset.Find(x => x.Name == Trans(selectedValue));
            g.ItemsSource = currentItem.Injs;
            
            Title = "Method Editor -- " + projName + "--" + selectedValue;
            CIndex = cIndex;
            StartVial = startVial;
        }
        private void grid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (r2.IsChecked == true)
            {
                StartVial[CIndex] = 1;
            }
            else
            {
                StartVial[CIndex] = 0;
            }
            ParentWindow.IsEnabled = true;
            ParentWindow.Show();
        }
        public static ItemCategory Trans(string selectedValue)
        {
            switch (selectedValue)
            {
                case "含量":
                    return ItemCategory.C;
                case "含量均匀度":
                    return ItemCategory.U;
                case "溶出度":
                    return ItemCategory.D;
                case "有关物质":
                    return ItemCategory.R;
                case "耐酸力":
                    return ItemCategory.T;
                default:
                    throw new Exception();

            }
        }
    }
}
