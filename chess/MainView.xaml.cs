using chess;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace uwp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginView loginView = new LoginView();
        RegistrationView registrationView = new RegistrationView();
        ForgotPasswordView forgotPasswordView = new ForgotPasswordView();
        GameView boardView = new GameView();
        MenuView menuView = new MenuView();

        public MainWindow()
        {
            InitializeComponent();
            
            MainContent.Content = loginView;

            loginView.RequestChangeContent += RegistrationView_RequestChangeContent;
            registrationView.RequestChangeContent += RegistrationView_RequestChangeContent;
            forgotPasswordView.RequestChangeContent += RegistrationView_RequestChangeContent;
            menuView.RequestChangeContent += RegistrationView_RequestChangeContent;
            boardView.RequestChangeContent += RegistrationView_RequestChangeContent;
        }

        private void ForgotPasswordView_RequestChangeContent(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RegistrationView_RequestChangeContent(object sender, EventArgs e)
        {
            WindowEventArgs args = e as WindowEventArgs;

            if (args.windowName == "registrationWindow")
            {
                MainContent.Content = registrationView;
            }
            else if (args.windowName == "forgotPasswordWindow")
            {
                MainContent.Content = forgotPasswordView;
            }
            else if (args.windowName == "loginWindow")
            {
                MainContent.Content = loginView;
            }
            else if (args.windowName == "boardWindow")
            {
                if (menuView.isNewGame)
                {
                    boardView = new GameView();
                    boardView.RequestChangeContent += RegistrationView_RequestChangeContent;

                    menuView.isNewGame = false;
                }
                if (!boardView.isGameStarted)
                {
                    boardView.isSecondPlayerMove = menuView.IsSecondPlayerMove;
                    boardView.UpdateColor();
                }
                MainContent.Content = boardView;
            }
            else if (args.windowName == "menuWindow")
            {
                MainContent.Content = menuView;
            }
        }
    }
}
