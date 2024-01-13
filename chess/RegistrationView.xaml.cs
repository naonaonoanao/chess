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
            RegUsernameTextBox.TextChanged += UsernameTextBox_TextChanged;
        }

        private const int МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ = 20;
        private const int МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ = 12;

        private const int МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА = 25;
        private const int МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА = 12;

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
            // Проверка ограничения по количеству символов
            if (RegPasswordTextBox.Password.Length > МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
            {
                ShowErrorPopupPassword("Слишком длинный пароль");
            }
            else if (RegPasswordTextBox.Password.Length < МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
            {
                ShowErrorPopupPassword("Слишком короткий пароль");
            }
            else
            {
                HideErrorPopupPassword();
            }
        }

        private void ShowErrorPopupPassword(string errorMessage)
        {
            ErrorMessage.Text = errorMessage;
            ErrorPopupPassword.IsOpen = true;
        }

        private void HideErrorPopupPassword()
        {
            ErrorMessage.Text = string.Empty;
            ErrorPopupPassword.IsOpen = false;
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
            // Проверка ограничения по количеству символов
            if (RegRepeatPasswordTextBox.Password.Length > МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
            {
                ShowErrorPopupPasswordRepeat("Слишком длинный пароль");
            }

            else if (RegRepeatPasswordTextBox.Password.Length < МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
            {
                ShowErrorPopupPasswordRepeat("Слишком короткий пароль");
            }
            else
            {
                HideErrorPopupPasswordRepeat();
            }
        }

        private void ShowErrorPopupPasswordRepeat(string errorMessage)
        {
            ErrorMessage.Text = errorMessage;
            ErrorPopupPasswordRepeat.IsOpen = true;
        }

        private void HideErrorPopupPasswordRepeat()
        {
            ErrorMessage.Text = string.Empty;
            ErrorPopupPasswordRepeat.IsOpen = false;
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox RegUsernameTextBox = sender as TextBox;
            // Проверка ограничения по количеству символов
            if (RegUsernameTextBox.Text.Length > МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА)
            {
                ShowErrorPopupLogin("Слишком длинный логин");
            }

            else if (RegRepeatPasswordTextBox.Password.Length < МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА)
            {
                ShowErrorPopupLogin("Слишком короткий логин");
            }
            else
            {
                HideErrorPopupLogin();
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

        private void ShowErrorPopupLogin(string errorMessage)
        {
            ErrorMessage.Text = errorMessage;
            ErrorPopupLogin.IsOpen = true;
        }

        private void HideErrorPopupLogin()
        {
            ErrorMessage.Text = string.Empty;
            ErrorPopupLogin.IsOpen = false;
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
                ErrorMessage.Text = "Подтвердите, что вы прочитали соглашение!";
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
