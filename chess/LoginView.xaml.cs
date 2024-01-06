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
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public event EventHandler RequestChangeContent;
        private int failedLoginAttempts = 0;
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock loginTextBlock = PasswordBox.Template.FindName("LoginTextBlock", PasswordBox) as TextBlock;
            if (PasswordBox.Password.Length == 0)
            {
                loginTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                loginTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var userRepository = new UserRepository();
            string username = UsernameTextBox.Text;
            string password = new NetworkCredential(string.Empty, PasswordBox.SecurePassword).Password;

            if (userRepository.VerifyUser(username, password))
            {
                failedLoginAttempts = 0;
                ErrorMessage.Text = "Вход успешен";
                ErrorMessage.Visibility = Visibility.Visible;

                // Временный переход напрямую на доску

                UsernameTextBox.Clear();
                PasswordBox.Clear();
                RememberPasswordBox.IsChecked = false;

                ErrorMessage.Visibility = Visibility.Collapsed;

                string windowName = "boardWindow";
                WindowEventArgs args = new WindowEventArgs(windowName);

                RequestChangeContent?.Invoke(this, args);
            }
            else
            {
                failedLoginAttempts++;
                ErrorMessage.Text = "Неправильный логин или пароль";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void RegistrationWindow(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Clear();
            PasswordBox.Clear();
            RememberPasswordBox.IsChecked = false;

            ErrorMessage.Visibility = Visibility.Collapsed;

            string windowName = "registrationWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void ForgotPasswordWindow(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Clear();
            PasswordBox.Clear();
            RememberPasswordBox.IsChecked = false;

            ErrorMessage.Visibility = Visibility.Collapsed;

            string windowName = "forgotPasswordWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }
    }
}
