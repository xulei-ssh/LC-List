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
    /// MethodEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MethodEditor : Window
    {
        Dictionary<string, ProjectInfo> database;

        public MethodEditor()
        {
            InitializeComponent();
            database = ConfigParser.Parse(@"D:\c");
            cProj.Items.Clear();
            database.Keys.ToList().ForEach(x => cProj.Items.Add(x));
        }

        private void cProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fullProt= database[cProj.SelectedValue.ToString()].Protocol;
            tProt.Text = fullProt.Split(' ')[0];
            tVer.Text = fullProt.Split(' ')[1].Split('.')[1];
            //clear all others
            cItem.SelectedIndex = -1;
            tSTD.Text = "";
            tCondition.Text = "";
            cConfig.Text = "";
            cItem.ItemsSource = database[cProj.SelectedValue.ToString()].Items.ConvertAll(x => x.Name);
        }

        private void cItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cItem.SelectedIndex != -1)
            {
                tCondition.Text = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].LCCondition.ToString();
                if (cItem.SelectedValue.ToString() == "Related Substance")
                {
                    tSTD.IsEnabled = false;
                    tSTD.Text = "";
                    cConfig.IsEnabled = true;
                    cConfig.Items.Add("self");
                    cConfig.Items.Add("union");
                    cConfig.SelectedIndex = database[cProj.SelectedValue.ToString()]["Related Substance"].Config == "self" ? 0 : 1;
                }
                else
                {
                    tSTD.IsEnabled = true;
                    tSTD.Text = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].StdType.ToString();
                    cConfig.IsEnabled = false;
                    cConfig.Items.Clear();
                }
                g.ItemsSource = database[cProj.SelectedValue.ToString()][cItem.SelectedValue.ToString()].Injs;


            }
        }
    }
}
