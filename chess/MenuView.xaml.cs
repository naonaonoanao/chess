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
        public bool isNewGame = true;
        public MenuView()
        {
            InitializeComponent();
        }


        private bool isButtonHighlighted = false;


        private Button lastClickedButton = null;

        private void BlackColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Black color clicked");
            IsSecondPlayerMove = true;

            // Переключаем состояние подсветки кнопки
            ToggleHighlight(blackColorButton);
        }

        private void WhiteColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("White color clicked");
            IsSecondPlayerMove = false;

            // Переключаем состояние подсветки кнопки
            ToggleHighlight(whiteColorButton);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && lastClickedButton != button)
            {
                // Применяем подсветку при наведении курсора, если кнопка не была нажата
                SolidColorBrush borderBrush = new SolidColorBrush(Colors.Yellow);
                button.BorderBrush = borderBrush;
                button.BorderThickness = new Thickness(2); // Толщина рамки
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && lastClickedButton != button && !isButtonHighlighted)
            {
                // Очищаем подсветку при уходе курсора, если кнопка не была нажата и не подсвечена
                button.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void ToggleHighlight(Button button)
        {
            if (lastClickedButton != null && lastClickedButton != button)
            {
                // Снимаем подсветку с предыдущей кнопки, если она была нажата и не совпадает с текущей
                isButtonHighlighted = false;
                lastClickedButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            if (lastClickedButton == button)
            {
                // Если кнопка была нажата ранее, снимаем подсветку
                isButtonHighlighted = false;
                lastClickedButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
                lastClickedButton = null;
            }
            else
            {
                // Если кнопка не была нажата ранее, применяем подсветку
                isButtonHighlighted = true;
                SolidColorBrush borderBrush = new SolidColorBrush(Colors.Yellow);
                button.BorderBrush = borderBrush;
                lastClickedButton = button;
            }
        }





        private void Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Difficulty changed");
            // Здесь добавьте свою логику для обработки изменения уровня сложности
        }

        private void CreateGame_Click(object sender, RoutedEventArgs e)
        {
            isNewGame = true;

            string windowName = "boardWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            string windowName = "boardWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        private void BackToLoginWindow(object sender, RoutedEventArgs e)
        {
            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }
    }
}
