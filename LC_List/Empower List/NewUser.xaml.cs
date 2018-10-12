using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
namespace Empower_List
{
    public partial class NewUser : Window
    {
        Auth Parent { get; set; }
        public NewUser(Auth parent)
        {
            InitializeComponent();
            Parent = parent;
            lblName.Content = Parent.tName.Text;
            pass.Focus();
        }
        protected override void OnClosing(CancelEventArgs e) => Parent.IsEnabled = true;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (pass.Password == Parent.tPass.Password)
            {
                ConfigParser.AddToken(Parent.tName.Text, pass.Password);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Password not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                pass.Focus();
            }
        }
        private void pass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnOK_Click(null, null);
        }
    }
}