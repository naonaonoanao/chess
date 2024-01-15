using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
