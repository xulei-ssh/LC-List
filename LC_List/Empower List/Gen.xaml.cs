using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;

namespace Empower_List
{
    public partial class Gen : Window
    {
        new MainWindow Parent;
        public Gen(MainWindow parent)
        {
            Parent = parent;
            InitializeComponent();
            RefreshList(false);
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
                MessageBox.Show("无法识别的起始编号格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (seqList.Items.Count == 0 || seqList.SelectedIndex == -1)
            {
                MessageBox.Show("请选择文件", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Sequences\\" + seqList.SelectedItem.ToString() + ".elw"))
            {
                MessageBox.Show("找不到所需的文件", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ConfigParser.GenReport(AppDomain.CurrentDomain.BaseDirectory + "Sequences\\" + seqList.SelectedItem.ToString() + ".elw", startIndex, tFont.Text == "" ? "楷体_GB2312" : tFont.Text);
            Close();
        }

        private void tIndex_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            tIndex.PreviewKeyUp -= tIndex_PreviewKeyUp;
            if (e.Key == Key.Enter) btnOK_Click(this, null);
        }

        private void seqList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOK_Click(this, null);
        }

        private void tIndex_KeyUp(object sender, KeyEventArgs e)
        {
            if (seqList.SelectedIndex != -1 && e.Key == Key.Enter)
            {
                btnOK_Click(this, null);
            }
        }

        private void radioAll_Checked(object sender, RoutedEventArgs e)
        {
            radioRecent.IsChecked = false;
            RefreshList(true);
        }

        private void radioRecent_Checked(object sender, RoutedEventArgs e)
        {
            radioAll.IsChecked = false;
            RefreshList(false);
        }
        private void RefreshList(bool isAll)
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Sequences");
            List<string> list = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (isAll || (!isAll && DateTime.Now - File.GetCreationTime(files[i]) <= TimeSpan.FromDays(10)))
                {
                    list.Add(files[i].Substring(files[i].LastIndexOf("\\") + 1, files[i].Length - files[i].LastIndexOf("\\") - 5));
                }
            }
            list.Sort(new FileDateComparer());
            seqList.ItemsSource = list;
        }

    }
    public class FileDateComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return -(int)(File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "Sequences\\" + x + ".elw") - File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "Sequences\\" + y + ".elw")).TotalMilliseconds;
        }
    }

}

