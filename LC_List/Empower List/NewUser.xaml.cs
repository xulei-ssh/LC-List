using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
namespace Empower_List
{
    public partial class NewUser : Window
    {
        new Auth Parent { get; set; }
        bool swt = false;
        public NewUser(Auth parent)
        {
            InitializeComponent();
            Parent = parent;
            btnOK.Click += btnOK_Click;

            lblName.Content = Parent.tName.Text;
            pass.Focus();
        }
        protected override void OnClosing(CancelEventArgs e) => Parent.IsEnabled = true;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (pass.Password == Parent.tPass.Password)
            {
                ConfigParser.ChangeToken(Parent.tName.Text, pass.Password);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Password not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                pass_LostFocus(null, null);
                pass.Focus();
            }
        }
        private void pass_KeyUp(object sender, KeyEventArgs e)
        {
            if (swt && e.Key == Key.Enter)
            {
                btnOK_Click(null, null);
            }
            else
            {
                swt = true;
            }
        }

        private void pass_GotFocus(object sender, RoutedEventArgs e)
        {
            pass.KeyUp += pass_KeyUp;
        }

        private void pass_LostFocus(object sender, RoutedEventArgs e)
        {
            pass.KeyUp -= pass_KeyUp;

        }
    }
}