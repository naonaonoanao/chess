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

        private const int maxQualityCharPassword = 20;
        private const int minQualityCharPassword = 8;

        private const int maxQualityCharLogin = 25;
        private const int minQualityCharLogin = 8;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock RegPasswordTextBlock = RegPasswordTextBox.Template.FindName("LoginTextBlock", RegPasswordTextBox) as TextBlock;
            if (RegPasswordTextBox.Password.Length == 0)
            {
                RegPasswordTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                RegPasswordTextBlock.Visibility = Visibility.Collapsed;
            }
            // Проверка ограничения по количеству символов
            if (RegPasswordTextBox.Password.Length > maxQualityCharPassword)
            {
                ShowErrorPopupPassword("Слишком длинный пароль");
            }
            else if (RegPasswordTextBox.Password.Length < minQualityCharPassword)
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
            TextBlock RegRepeatPasswordTextBlock = RegRepeatPasswordTextBox.Template.FindName("LoginTextBlock", RegRepeatPasswordTextBox) as TextBlock;
            if (RegRepeatPasswordTextBox.Password.Length == 0)
            {
                RegRepeatPasswordTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                RegRepeatPasswordTextBlock.Visibility = Visibility.Collapsed;
            }
            // Проверка ограничения по количеству символов
            if (RegRepeatPasswordTextBox.Password.Length > maxQualityCharPassword)
            {
                ShowErrorPopupPasswordRepeat("Слишком длинный пароль");
            }

            else if (RegRepeatPasswordTextBox.Password.Length < minQualityCharPassword)
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
            if (RegUsernameTextBox.Text.Length > maxQualityCharLogin)
            {
                ShowErrorPopupLogin("Слишком длинный логин");
            }

            else if (RegUsernameTextBox.Text.Length < minQualityCharLogin)
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

            else
            {
                // Отображение соответствующих окон ошибок
                if (isLoginError && isPasswordError)
                {
                    ErrorMessage.Text = "Недопустимая длина логина или пароля";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else if (isLoginError)
                {
                    ErrorMessage.Text = "Недопустимая длина логина";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else if (isPasswordError)
                {
                    ErrorMessage.Text = "Недопустимая длина пароля";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else
                {
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
    }
}
