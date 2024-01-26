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

namespace uwp
{
    public partial class ForgotPasswordView : UserControl
    {
        public event EventHandler RequestChangeContent;
        public ForgotPasswordView()
        {
            InitializeComponent();
            RegUsernameTextBox.TextChanged += RegUsernameTextBox_TextChanged;
        }

        private const int maxQualityCharLogin = 25;
        private const int minQualityCharLogin = 8;

        private void RegUsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox regUsernameTextBox = sender as TextBox;

            // Проверка ограничения по количеству символов
            if (regUsernameTextBox.Text.Length > maxQualityCharLogin)
            {
                ShowErrorPopupSend("Недопустимое количество символов");
            }
            else if (regUsernameTextBox.Text.Length < minQualityCharLogin)
            {
                ShowErrorPopupSend("Недопустимое количество символов");
            }
            else
            {
                HideErrorPopupSend();
            }
        }
        private void ShowErrorPopupSend(string errorMessage)
        {
            ErrorMessage.Text = errorMessage;
            ErrorPopupSend.IsOpen = true;
        }

        private void HideErrorPopupSend()
        {
            ErrorMessage.Text = string.Empty;
            ErrorPopupSend.IsOpen = false;
        }

        private void BackToLoginWindow(object sender, RoutedEventArgs e)
        {
            RegUsernameTextBox.Clear();
            ErrorMessage.Visibility = Visibility.Collapsed;

            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string username = RegUsernameTextBox.Text;

            bool isLoginError = false;

            // Проверка длины логина
            if (username.Length > maxQualityCharLogin || username.Length < minQualityCharLogin)
            {
                isLoginError = true;
            }

            if (username == string.Empty)
            {
                ErrorMessage.Text = "Введите почту!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

            else
            {
                if (isLoginError)
                {
                    ErrorMessage.Text = "Недопустимое количество символов.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    try
                    {
                        ErrorMessage.Text = "Инструкция по восстановлению пароля отправлена на почту.";
                        ErrorMessage.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage.Text = "Инструкция уже отправлена. Проверьте вашу почту.";
                        ErrorMessage.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
