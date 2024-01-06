using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using uwp;

namespace uwp
{
    public partial class RegistrationView : UserControl
    {
        public event EventHandler RequestChangeContent;
        public RegistrationView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock loginTextBlock = RegPasswordTextBox.Template.FindName("LoginTextBlock", RegPasswordTextBox) as TextBlock;
            if (RegPasswordTextBox.Password.Length == 0)
            {
                loginTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                loginTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock loginTextBlock = RegRepeatPasswordTextBox.Template.FindName("LoginTextBlock", RegRepeatPasswordTextBox) as TextBlock;
            if (RegRepeatPasswordTextBox.Password.Length == 0)
            {
                loginTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                loginTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginWindow(object sender, RoutedEventArgs e)
        {
            RegPasswordTextBox.Clear();
            RegRepeatPasswordTextBox.Clear();
            RegUsernameTextBox.Clear();
            conditionsBox.IsChecked = false;

            TextBlock loginTextBlock = RegRepeatPasswordTextBox.Template.FindName("LoginTextBlock", RegRepeatPasswordTextBox) as TextBlock;
            ErrorMessage.Visibility = Visibility.Collapsed;

            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            var userRepository = new UserRepository();
            string username = RegUsernameTextBox.Text;
            string password = new NetworkCredential(string.Empty, RegPasswordTextBox.SecurePassword).Password;
            string repeatPassword = new NetworkCredential(string.Empty, RegRepeatPasswordTextBox.SecurePassword).Password;

            if (password != repeatPassword)
            {
                ErrorMessage.Text = "Пароли не совпадают!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            if (username == string.Empty)
            {
                ErrorMessage.Text = "Введите логин!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            if (password == string.Empty)
            {
                ErrorMessage.Text = "Введите пароль!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            if (password != repeatPassword)
            {
                ErrorMessage.Text = "Пароли не совпадают!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            if (!(bool)conditionsBox.IsChecked)
            {
                ErrorMessage.Text = "Подведтите, что вы прочитали соглашение!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            try
            {
                userRepository.AddUser(username, password);
                ErrorMessage.Text = "Регистрация успешна";
                ErrorMessage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Вы уже зарегистрированы!";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
