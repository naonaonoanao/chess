using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public event EventHandler RequestChangeContent;
        public bool IsSecondPlayerMove = false;
        public MenuView()
        {
            InitializeComponent();
        }

        private void BlackColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Black color clicked");
            IsSecondPlayerMove = true;
            // Здесь добавьте свою логику для обработки клика на черный цвет
        }

        private void WhiteColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("White color clicked");
            IsSecondPlayerMove = false;
            // Здесь добавьте свою логику для обработки клика на белый цвет
        }

        private void Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Difficulty changed");
            // Здесь добавьте свою логику для обработки изменения уровня сложности
        }

        private void CreateGame_Click(object sender, RoutedEventArgs e)
        {
            string windowName = "boardWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Continue game clicked");
            // Здесь добавьте свою логику для продолжения игры
        }

        private void BackToLoginWindow(object sender, RoutedEventArgs e)
        {
            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }
    }
}
