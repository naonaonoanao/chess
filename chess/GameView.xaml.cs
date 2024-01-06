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
    public partial class GameView : UserControl
    {
        private Border selectedCell; // Переменная для хранения выбранной ячейки

        private void ChessGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Получаем элемент, на который было нажатие
            UIElement element = e.OriginalSource as UIElement;

            // Проверяем, является ли элемент ячейкой (Border)
            if (element is Border clickedCell)
            {
                // Проверяем, содержит ли ячейка TextBlock с фигурой
                if (clickedCell.Child is TextBlock textBlock && !string.IsNullOrEmpty(textBlock.Text.Trim()))
                {
                    // Если фигура уже выбрана, снимаем выделение
                    if (selectedCell != null)
                    {
                        textBlock.Text = (selectedCell.Child as TextBlock)?.Text; // Перемещаем фигуру
                        (selectedCell.Child as TextBlock)?.ClearValue(TextBlock.TextProperty); // Очищаем старую ячейку
                        selectedCell = null; // Сбрасываем выбранную ячейку
                    }
                    else
                    {
                        // Если фигура еще не выбрана, запоминаем текущую ячейку
                        selectedCell = clickedCell;
                    }
                }
            }
        }

    }
}