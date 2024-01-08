using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace uwp
{
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private TextBlock? selectedCell;
        private Style? selectedPrevStyle;

        private void ChessGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = e.OriginalSource as UIElement;

            if (element is TextBlock)
            {
                ReturnPrevSelectedStyle();
                if (selectedCell == null)
                {
                    selectedCell = (TextBlock)element;
                    HighlightSelectedCell();
                }
                else
                {
                    MovePiece((TextBlock)element);
                }
            }
        }

        private void HighlightSelectedCell()
        {
            if (selectedCell != null && selectedCell.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCell.Parent;
                selectedPrevStyle = selectedBorder.Style;
                selectedBorder.Style = ChessGrid.Resources["ChessCellSelected"] as Style;
            }
        }

        private void ReturnPrevSelectedStyle()
        {
            if (selectedCell != null && selectedCell.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCell.Parent;
                selectedBorder.Style = selectedPrevStyle;
            }
        }

        private void MovePiece(TextBlock targetCell)
        {
            int fromRow = Grid.GetRow((UIElement)selectedCell.Parent);
            int fromColumn = Grid.GetColumn((UIElement)selectedCell.Parent);
            int toRow = Grid.GetRow((UIElement)targetCell.Parent);
            int toColumn = Grid.GetColumn((UIElement)targetCell.Parent);

            if (IsValidPawnMove(fromRow, fromColumn, toRow, toColumn, targetCell))
            {
                SwapPieces(selectedCell, targetCell);
            }

            selectedCell = null;
        }

        private bool IsValidPawnMove(int fromRow, int fromColumn, int toRow, int toColumn, TextBlock targetCell)
        {
            char selectedPiece = selectedCell.Text[0];

            
            // Проверка хода для белой пешки
            if (selectedPiece == '♙')
            {
                if (toColumn == fromColumn && toRow == fromRow + 1 && targetCell.Text == " ")
                {
                    return true;
                }

                if (fromRow == 1 && toColumn == fromColumn && toRow == fromRow + 2 && targetCell.Text == " ")
                {
                    return true;
                }

                if (toRow == fromRow + 1 && (toColumn == fromColumn - 1 || toColumn == fromColumn + 1) && targetCell.Text != " " && IsBlackPiece(targetCell))
                {
                    return true;
                }
            }
            // Проверка хода для черной пешки
            else if (selectedPiece == '♟')
            {
                if (toColumn == fromColumn && toRow == fromRow + 1 && targetCell.Text == " ")
                {
                    return true;
                }

                if (fromRow == 1 && toColumn == fromColumn && toRow == fromRow + 2 && targetCell.Text == " " && GetSymbolAt(fromRow + 1, toColumn) == ' ')
                {
                    return true;
                }

                if (toRow == fromRow + 1 && (toColumn == fromColumn - 1 || toColumn == fromColumn + 1) && targetCell.Text != " " && IsWhitePiece(toRow, toColumn))
                {
                    return true;
                }
            }

            return false;
        }

        private void SwapPieces(TextBlock sourceCell, TextBlock targetCell)
        {
            var oldForeground = sourceCell.Foreground;
            sourceCell.Foreground = targetCell.Foreground;
            targetCell.Foreground = oldForeground;

            targetCell.Text = sourceCell.Text;
            sourceCell.Text = " ";
        }

        private char GetSymbolAt(int row, int column)
        {
            return ((TextBlock)ChessGrid.Children[row * 8 + column]).Text[0];
        }

        private bool IsWhitePiece(int row, int column)
        {
            return ((TextBlock)ChessGrid.Children[row * 8 + column]).Foreground == Brushes.White;
        }

        private bool IsBlackPiece(TextBlock targetCell)
        {
            return targetCell.Foreground == Brushes.Black;
        }
    }
}
