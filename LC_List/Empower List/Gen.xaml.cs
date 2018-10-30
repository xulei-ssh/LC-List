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
using System.IO;
using System.ComponentModel;

namespace Empower_List
{
    /// <summary>
    /// Gen.xaml 的交互逻辑
    /// </summary>
    public partial class Gen : Window
    {
        MainWindow Parent;
        public Gen(MainWindow parent)
        {
            Parent = parent;
            InitializeComponent();
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Sequences");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Substring(files[i].LastIndexOf("\\") + 1, files[i].Length - files[i].LastIndexOf("\\") - 5);
            }
            List<string> list = files.ToList();
            list.Sort();
            seqList.ItemsSource = list;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
            base.OnClosing(e);
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = 1;
            if (!(int.TryParse(tIndex.Text == "" ? "1" : tIndex.Text, out startIndex) && startIndex > 0))
            {
                MessageBox.Show("Invalid starting index.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (seqList.Items.Count == 0 || seqList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a list file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ConfigParser.GenReport(AppDomain.CurrentDomain.BaseDirectory + "Sequences\\" + seqList.SelectedItem.ToString() + ".elw", startIndex);
            Close();
        }

        private void tIndex_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnOK_Click(this, null);
        }

        private void seqList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOK_Click(this, null);
        }
    }

}

