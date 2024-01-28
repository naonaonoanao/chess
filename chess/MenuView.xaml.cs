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
    public partial class MenuView : UserControl
    {
        public event EventHandler RequestChangeContent;
        public bool IsSecondPlayerMove = false;
        public bool isNewGame = true;
        public MenuView()
        {
            InitializeComponent();
        }


        public bool isButtonHighlighted = false;
        public Button lastClickedButton = null;


        public void BlackColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Black color clicked");
            IsSecondPlayerMove = true;

            // Переключаем состояние подсветки кнопки
            ToggleHighlight(blackColorButton);
        }

        public void WhiteColor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("White color clicked");
            IsSecondPlayerMove = false;

            // Переключаем состояние подсветки кнопки
            ToggleHighlight(whiteColorButton);
        }

        public void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && lastClickedButton != button)
            {
                // Применяем подсветку при наведении курсора, если кнопка не была нажата
                SolidColorBrush borderBrush = new SolidColorBrush(Colors.Yellow);
                button.BorderBrush = borderBrush;
                button.BorderThickness = new Thickness(2);
            }
        }

        public void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && lastClickedButton != button && !isButtonHighlighted)
            {
                // Очищаем подсветку при уходе курсора, если кнопка не была нажата и не подсвечена
                button.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        public void ToggleHighlight(Button button)
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

        public bool isMinimax = true;
        public void Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedDifficulty = selectedItem.Content.ToString();
                    
                    if (selectedDifficulty == "Легкий")
                    {
                        isMinimax = false;
                    }
                    else if (selectedDifficulty == "Сложный")
                    {
                        isMinimax = true;
                    }
                }
            }
        }

        public void CreateGame_Click(object sender, RoutedEventArgs e)
        {
            bool isColorSelected = isButtonHighlighted;
            bool isDifficultySelected = ((ComboBoxItem)difficultyComboBox.SelectedItem)?.Content != null;

            if (!isColorSelected && !isDifficultySelected)
            {
                ErrorMessage.Text = "Выберите цвет и уровень сложности";
                ErrorMessage.Visibility = Visibility.Visible;
            }
            else if (!isColorSelected)
            {
                ErrorMessage.Text = "Выберите цвет";
                ErrorMessage.Visibility = Visibility.Visible;
            }
            else if (!isDifficultySelected)
            {
                ErrorMessage.Text = "Выберите уровень сложности";
                ErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                ResetColorSelection();

                ResetDifficultySelection();

                ErrorMessage.Visibility = Visibility.Hidden;
                isNewGame = true;

                string windowName = "boardWindow";
                WindowEventArgs args = new WindowEventArgs(windowName);

                RequestChangeContent?.Invoke(this, args);
            }
        }

        public void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            ResetColorSelection();

            ResetDifficultySelection();

            ErrorMessage.Visibility = Visibility.Hidden;
            string windowName = "boardWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        public void BackToLoginWindow(object sender, RoutedEventArgs e)
        {
            ResetColorSelection();

            ResetDifficultySelection();

            ErrorMessage.Visibility = Visibility.Hidden;
            string windowName = "loginWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }

        public void ResetColorSelection()
        {
            isButtonHighlighted = false;
            lastClickedButton = null;

            // Снимаем подсветку с кнопок выбора цвета
            SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);
            blackColorButton.BorderBrush = transparentBrush;
            whiteColorButton.BorderBrush = transparentBrush;
        }

        public void ResetDifficultySelection()
        {
            difficultyComboBox.SelectedIndex = -1;
        }
    }
}
