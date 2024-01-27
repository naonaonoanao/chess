using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class LoginView : UserControl
    {
        public event EventHandler RequestChangeContent;
        private int failedLoginAttempts = 0;
        public LoginView()
        {
            InitializeComponent();
            UsernameTextBox.TextChanged += UsernameTextBox_TextChanged;
        }

        private const int maxQualityCharPassword = 20;
        private const int minQualityCharPassword = 8;

        private const int maxQualityCharLogin = 25;
        private const int minQualityCharLogin = 8;

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
            // Проверка ограничения по количеству символов
            if (PasswordBox.Password.Length > maxQualityCharPassword)
            {
                ShowErrorPopupPassword("Слишком длинный пароль");
            } 
            else if (PasswordBox.Password.Length < minQualityCharPassword)
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

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox usernameTextBox = sender as TextBox;

            // Проверка ограничения по количеству символов
            if (usernameTextBox.Text.Length > maxQualityCharLogin)
            {
                ShowErrorPopupLogin("Слишком длинный логин");
            }
            else if (usernameTextBox.Text.Length < minQualityCharLogin)
            {
                ShowErrorPopupLogin("Слишком короткий логин");
            }
            else
            {
                HideErrorPopupLogin();
            }
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

        public string login;
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var userRepository = new UserRepository();
            string username = UsernameTextBox.Text;
            string password = new NetworkCredential(string.Empty, PasswordBox.SecurePassword).Password;

            // Переменные для отслеживания статуса ошибок
            bool isLoginError = false;
            bool isPasswordError = false;

            // Проверка длины логина
            if (username.Length > maxQualityCharLogin || username.Length < minQualityCharLogin)
            {
                isLoginError = true;
            }

            // Проверка длины пароля
            if (password.Length > maxQualityCharPassword || password.Length < minQualityCharPassword)
            {
                isPasswordError = true;
            }

            if (username == string.Empty)
            {
                ShowErrorPopupLogin("Введите логин!");
                return;
            }

            if (password == string.Empty)
            {
                ShowErrorPopupPassword("Введите пароль!");
                return;
            }

            else
            {
                // Отображение соответствующих окон ошибок
                if (isLoginError && isPasswordError)
                {
                    ErrorMessage.Text = "Недопустимая длина логина или пароля.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else if (isLoginError)
                {
                    ErrorMessage.Text = "Недопустимая длина логина.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else if (isPasswordError)
                {
                    ErrorMessage.Text = "Недопустимая длина пароля.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    if (userRepository.VerifyUser(username, password))
                    {
                        login = username;
                        failedLoginAttempts = 0;
                        ErrorMessage.Text = "Вход успешен.";
                        ErrorMessage.Visibility = Visibility.Visible;

                        // переход на меню
                        UsernameTextBox.Clear();
                        PasswordBox.Clear();
                        RememberPasswordBox.IsChecked = false;

                        ErrorMessage.Visibility = Visibility.Collapsed;

                        string windowName = "menuWindow";
                        WindowEventArgs args = new WindowEventArgs(windowName);

                        RequestChangeContent?.Invoke(this, args);
                    }
                    else
                    {
                        failedLoginAttempts++;
                        ErrorMessage.Text = "Неправильный логин или пароль.";
                        ErrorMessage.Visibility = Visibility.Visible;
                    }

                }
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
