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
                MessageBox.Show("Invalid Name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (!CheckFile(tName.Text.Trim()))
            {
                MessageBox.Show("Name already occupied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (tName.Text.Trim().Contains("\\")|| tName.Text.Trim().Contains("/") || tName.Text.Trim().Contains(":") || tName.Text.Trim().Contains("*") || tName.Text.Trim().Contains("?") || tName.Text.Trim().Contains("\"") || tName.Text.Trim().Contains("<") || tName.Text.Trim().Contains(">") || tName.Text.Trim().Contains("|"))
            {
                var r = MessageBox.Show("Name cannot contain the following:\n\\ / : * ? \" < > |", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tName.Focus();
                return;
            }
            if (ConfigParser.SaveList(ProjName, ItemsDone, Items, StdSuffix, Lists, Lots, tName.Text.Trim()))
            {
                MessageBox.Show("List saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Parent.IsEnabled = true;
                Parent.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Save operation failed due to unknown reasons.", "Save failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
