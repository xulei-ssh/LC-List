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

namespace Empower_List
{
    /// <summary>
    /// AddProj.xaml 的交互逻辑
    /// </summary>
    public partial class AddItem : Window
    {
        new MethodEditor Parent { get; set; }
       ProjectInfo Project { get; set; }
        string ProjName { get; set; }
        public AddItem(MethodEditor me, ProjectInfo info,string projName)
        {
            InitializeComponent();
            Parent = me;
            Project = info;
            itemName.Items.Add("Related Substance");
            itemName.Items.Add("Assay");
            itemName.Items.Add("Content Uniformity");
            itemName.Items.Add("Dissolution");
            itemName.Items.Add("Acid Tolerance");
            stdType.IsEnabled = false;
            config.Items.Add("self");
            config.Items.Add("union");
            config.IsEnabled = false;
            lblProj.Content = projName + " " + info.Protocol;
            ProjName = projName;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (itemName.SelectedIndex == -1) return;
            foreach (var item in Project.Items)
            {
                if (item.Name == itemName.SelectedValue.ToString())
                {
                    MessageBox.Show("Item already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    config.KeyUp -= Enter_KeyUp;
                    stdType.KeyUp -= Enter_KeyUp;
                    return;
                }
            }
            if (!int.TryParse(condition.Text, out int temp1))
            {
                MessageBox.Show("Condition should be number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                config.KeyUp -= Enter_KeyUp;
                stdType.KeyUp -= Enter_KeyUp;
                condition.SelectAll();
                return;
            }
            if (itemName.SelectedValue.ToString() == "Related Substance")
            {
                if (config.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select Config.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    config.KeyUp -= Enter_KeyUp;
                    stdType.KeyUp -= Enter_KeyUp;
                    config.Focus();
                    return;
                }
            }
            else
            {
                if (!int.TryParse(stdType.Text, out int temp2))
                {
                    MessageBox.Show("STDType should be number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    config.KeyUp -= Enter_KeyUp;
                    stdType.KeyUp -= Enter_KeyUp;
                    stdType.SelectAll();
                    return;
                }
            }
            if (MessageBox.Show("Add " + itemName.SelectedValue.ToString() + " to "+ ProjName + " " + Project.Protocol+ " as a new item?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (itemName.SelectedValue.ToString() == "Related Substance")
                {
                    Project.Items.Add(new Item("Related Substance", int.Parse(condition.Text), 0, config.SelectedIndex == 0 ? "self" : "union"));
                }
                else
                {
                    Project.Items.Add(new Item(itemName.SelectedValue.ToString(), int.Parse(condition.Text), int.Parse(stdType.Text), ""));
                }
                Parent.cProj_SelectionChanged(null, null);
                Parent.cItem.SelectedIndex = Parent.cItem.Items.Count - 1;
                Close();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            Parent.IsEnabled = true;
            Parent.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Enter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(null, null);
            }
        }

        private void itemName_GotFocus(object sender, RoutedEventArgs e)
        {
            config.KeyUp -= Enter_KeyUp;
            stdType.KeyUp -= Enter_KeyUp;
            config.KeyUp += Enter_KeyUp;
            stdType.KeyUp += Enter_KeyUp;

        }

        private void itemName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemName_GotFocus(null, null);
            config.SelectedIndex = -1;
            if (itemName.SelectedValue.ToString() == "Related Substance")
            {
                stdType.IsEnabled = false;
                config.IsEnabled = true;
            }
            else
            {
                stdType.IsEnabled = true;
                config.IsEnabled = false;
            }
        }

        private void config_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemName_GotFocus(null, null);
        }
    }
}
