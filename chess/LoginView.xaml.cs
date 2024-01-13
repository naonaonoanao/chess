﻿using System;
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
            UsernameTextBox.TextChanged += UsernameTextBox_TextChanged;
        }

        private const int МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ = 20;
        private const int МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ = 12;

        private const int МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА = 25;
        private const int МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА = 12;

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
            if (PasswordBox.Password.Length > МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
            {
                ShowErrorPopupPassword("Слишком длинный пароль");
            }
            else
            {
                HideErrorPopupPassword();
            }
            if (PasswordBox.Password.Length < МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ПАРОЛЯ)
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
            if (usernameTextBox.Text.Length > МАКСИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА)
            {
                ShowErrorPopupLogin("Слишком длинный логин");
            }
            else if (usernameTextBox.Text.Length < МИНИМАЛЬНОЕ_КОЛИЧЕСТВО_СИМВОЛОВ_ЛОГИНА)
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var userRepository = new UserRepository();
            string username = UsernameTextBox.Text;
            string password = new NetworkCredential(string.Empty, PasswordBox.SecurePassword).Password;
            

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
