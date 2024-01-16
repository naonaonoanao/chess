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
        }


        private void BackToLoginWindow(object sender, RoutedEventArgs e)
        {
            RegUsernameTextBox.Clear();

            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string username = RegUsernameTextBox.Text;

            if (username == string.Empty)
            {
                ErrorMessage.Text = "Введите почту!";
                ErrorMessage.Visibility = Visibility.Visible;

                return;
            }

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
