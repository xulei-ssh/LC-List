using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;

namespace Empower_List
{
    public partial class SaveOptions : Window
    {
        new FinalList Parent;
        string ProjName;
        List<string> ItemsDone;
        List<Item> Items;
        Dictionary<int, string> StdSuffix;
        List<ObservableCollection<ListItem>> Lists;
        List<string> Lots;




        public SaveOptions(FinalList parent, string proj, List<string> itemsDone,List<Item> items, Dictionary<int, string> stdSuffix, List<ObservableCollection<ListItem>> lists, List<string> lots)
        {
            InitializeComponent();
            Parent = parent;
            ProjName = proj;
            Items = items;
            StdSuffix = stdSuffix;
            ItemsDone = itemsDone;
            Lists = lists;
            Lots = lots;
            tName.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Show();
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tName.Text.Trim().Length == 0)
            {
                MessageBox.Show("文件名无效", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (!CheckFile(tName.Text.Trim()))
            {
                MessageBox.Show("与已有文件重名，请重新命名", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (tName.Text.Trim().Contains(" ")||tName.Text.Trim().Contains("\\")|| tName.Text.Trim().Contains("/") || tName.Text.Trim().Contains(":") || tName.Text.Trim().Contains("*") || tName.Text.Trim().Contains("?") || tName.Text.Trim().Contains("\"") || tName.Text.Trim().Contains("<") || tName.Text.Trim().Contains(">") || tName.Text.Trim().Contains("|"))
            {
                var r = MessageBox.Show("文件名不能包含以下字符:\n\\ / : * ? \" < > | <空格>", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (ConfigParser.SaveList(ProjName, ItemsDone, Items, StdSuffix, Lists, Lots, tName.Text.Trim()))
            {
                MessageBox.Show("保存成功", "完成", MessageBoxButton.OK, MessageBoxImage.Information);
                Parent.IsEnabled = true;
                Parent.Show();
                Close();
            }
            else
            {
                MessageBox.Show("出现未预料的异常，无法保存。请联系管理员。", "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
            }

        }
        private bool CheckFile(string name)
        {
            string folderLoc = AppDomain.CurrentDomain.BaseDirectory + "Sequences";
            if (!Directory.Exists(folderLoc))
            {
                Directory.CreateDirectory(folderLoc);
                return true;
            }
            if (File.Exists(folderLoc + @"\" + name+".elw"))
            {
                return false;
            }
            return true;

        }

        private void tName_KeyDown(object sender, KeyEventArgs e)
        {
            tName.PreviewKeyDown -= tName_KeyDown;
            if (e.Key == Key.Enter)
            {
                btnSave_Click(null, null);
            }
        }
    }
}
