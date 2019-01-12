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
            lblName.Content = Parent.tName.Text;
            pass.Focus();
        }
        protected override void OnClosing(CancelEventArgs e) => Parent.IsEnabled = true;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (btnOK.IsEnabled)
            {
                ConfigParser.ChangeToken(Parent.tName.Text, pass.Password);
                DialogResult = true;
            }
        }
        private void pass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(null, null);
            }
        }
        private void pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pass.Password == Parent.tPass.Password)
            {
                lblTick.Content = "PASS";
                btnOK.IsEnabled = true;
            }
            else
            {
                lblTick.Content = "FAILED";
                btnOK.IsEnabled = false;
            }
        }
    }
}