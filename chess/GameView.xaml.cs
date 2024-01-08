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
        private bool IsWhiteTurn = true;

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

            if (IsValidMove(fromRow, fromColumn, toRow, toColumn, targetCell))
            {
                SwapPieces(selectedCell, targetCell);
            }

            selectedCell = null;
        }

        private bool IsValidMove(int fromRow, int fromColumn, int toRow, int toColumn, TextBlock targetCell)
        {
            char selectedPiece = selectedCell.Text[0];


            // Проверка хода для белой пешки
            if (IsWhiteTurn)
            {
                if (selectedPiece == '♙')
                {
                    if (toColumn == fromColumn && toRow == fromRow + 1 && targetCell.Text == " ")
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (fromRow == 1 && toColumn == fromColumn && toRow == fromRow + 2 && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (toRow == fromRow + 1 && (toColumn == fromColumn - 1 || toColumn == fromColumn + 1) && targetCell.Text != " " && IsBlackPiece(targetCell))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }
                }

                else if (selectedPiece == '♖')
                {
                    if (fromRow != toRow && toColumn == fromColumn && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (fromRow == toRow && toColumn != fromColumn && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (fromRow != toRow && toColumn == fromColumn && targetCell.Text != " " && IsBlackPiece(targetCell) && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (fromRow == toRow && toColumn != fromColumn && targetCell.Text != " " && IsBlackPiece(targetCell) && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = false;
                        return true;
                    }
                }

                else if (selectedPiece == '♔')
                {
                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && fromColumn == toColumn) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && fromRow == toRow) ||
                        ((toColumn == fromColumn - 1 && toRow == fromRow + 1) || (toColumn == fromColumn + 1 && toRow == fromRow - 1)) ||
                        ((toColumn == fromColumn + 1 && toRow == fromRow + 1) || (toColumn == fromColumn - 1 && toRow == fromRow - 1))) &&
                        (targetCell.Text == " ")
                       )
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && fromColumn == toColumn) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && fromRow == toRow) ||
                        ((toColumn == fromColumn - 1 && toRow == fromRow + 1) || (toColumn == fromColumn + 1 && toRow == fromRow - 1)) ||
                        ((toColumn == fromColumn + 1 && toRow == fromRow + 1) || (toColumn == fromColumn - 1 && toRow == fromRow - 1))) &&
                        (targetCell.Text != " ") &&
                        (IsBlackPiece(targetCell))
                       )
                    {
                        IsWhiteTurn = false;
                        return true;
                    }
                }

                else if (selectedPiece == '♘')
                {
                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && (toColumn == fromColumn - 2 || toColumn == fromColumn + 2)) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && (toRow == fromRow - 2 || toRow == fromRow + 2))) &&
                        (targetCell.Text == " ")
                       )
                    {
                        IsWhiteTurn = false;
                        return true;
                    }

                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && (toColumn == fromColumn - 2 || toColumn == fromColumn + 2)) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && (toRow == fromRow - 2 || toRow == fromRow + 2))) &&
                        (targetCell.Text != " ") &&
                        (IsBlackPiece(targetCell))
                       )
                    {
                        IsWhiteTurn = false;
                        return true;
                    }
                }
            }
            // Проверка хода для черной пешки
            else
            {
                if (selectedPiece == '♟')
                {
                    if (toColumn == fromColumn && toRow == fromRow - 1 && targetCell.Text == " ")
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (fromRow == 6 && toColumn == fromColumn && toRow == fromRow - 2 && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (toRow == fromRow - 1 && (toColumn == fromColumn - 1 || toColumn == fromColumn + 1) && targetCell.Text != " " && !IsBlackPiece(targetCell))
                    {
                        IsWhiteTurn = true;
                        return true;
                    }
                }

                else if (selectedPiece == '♜')
                {
                    if (fromRow != toRow && toColumn == fromColumn && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn)) 
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (fromRow == toRow && toColumn != fromColumn && targetCell.Text == " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn))
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (fromRow != toRow && toColumn == fromColumn && targetCell.Text != " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn) && !IsBlackPiece(targetCell))
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (fromRow == toRow && toColumn != fromColumn && targetCell.Text != " " && AreIntermediateCellsEmpty(fromRow, fromColumn, toRow, toColumn) && !IsBlackPiece(targetCell))
                    {
                        IsWhiteTurn = true;
                        return true;
                    }
                }

                else if (selectedPiece == '♚')
                {
                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && fromColumn == toColumn) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && fromRow == toRow) ||
                        ((toColumn == fromColumn - 1 && toRow == fromRow + 1) || (toColumn == fromColumn + 1 && toRow == fromRow - 1)) ||
                        ((toColumn == fromColumn + 1 && toRow == fromRow + 1) || (toColumn == fromColumn - 1 && toRow == fromRow - 1))) &&
                        (targetCell.Text == " ")
                       )
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && fromColumn == toColumn) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && fromRow == toRow) ||
                        ((toColumn == fromColumn - 1 && toRow == fromRow + 1) || (toColumn == fromColumn + 1 && toRow == fromRow - 1)) ||
                        ((toColumn == fromColumn + 1 && toRow == fromRow + 1) || (toColumn == fromColumn - 1 && toRow == fromRow - 1))) &&
                        (targetCell.Text != " ") &&
                        (!IsBlackPiece(targetCell))
                       )
                    {
                        IsWhiteTurn = true;
                        return true;
                    }
                }

                else if (selectedPiece == '♞')
                {
                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && (toColumn == fromColumn - 2 || toColumn == fromColumn + 2)) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && (toRow == fromRow - 2 || toRow == fromRow + 2))) &&
                        (targetCell.Text == " ")
                       )
                    {
                        IsWhiteTurn = true;
                        return true;
                    }

                    if (
                        (((toRow == fromRow + 1 || toRow == fromRow - 1) && (toColumn == fromColumn - 2 || toColumn == fromColumn + 2)) ||
                        ((toColumn == fromColumn + 1 || toColumn == fromColumn - 1) && (toRow == fromRow - 2 || toRow == fromRow + 2))) &&
                        (targetCell.Text != " ") &&
                        (!IsBlackPiece(targetCell))
                       )
                    {
                        IsWhiteTurn = true;
                        return true;
                    }
                }
            }



            return false;
        }

        private bool AreIntermediateCellsEmpty(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (fromRow != toRow && toColumn == fromColumn) // Вертикальное перемещение
            {
                int step = (toRow > fromRow) ? 1 : -1; // Определяем направление движения

                for (int row = fromRow + step; row != toRow; row += step)
                {
                    if (!IsCellEmpty(row, fromColumn))
                    {
                        // Промежуточная клетка занята, ход недопустим
                        return false;
                    }
                }
            }
            else if (fromRow == toRow && toColumn != fromColumn) // Горизонтальное перемещение
            {
                int step = (toColumn > fromColumn) ? 1 : -1; // Определяем направление движения

                for (int col = fromColumn + step; col != toColumn; col += step)
                {
                    if (!IsCellEmpty(fromRow, col))
                    {
                        // Промежуточная клетка занята, ход недопустим
                        return false;
                    }
                }
            }
            else if (Math.Abs(toRow - fromRow) == Math.Abs(toColumn - fromColumn)) // Диагональное перемещение
            {
                int rowStep = (toRow > fromRow) ? 1 : -1; // Определяем направление движения по вертикали
                int colStep = (toColumn > fromColumn) ? 1 : -1; // Определяем направление движения по горизонтали

                for (int row = fromRow + rowStep, col = fromColumn + colStep; row != toRow && col != toColumn; row += rowStep, col += colStep)
                {
                    if (!IsCellEmpty(row, col))
                    {
                        // Промежуточная клетка занята, ход недопустим
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsCellEmpty(int row, int column)
        {
            if (ChessGrid.Children.Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column) is Border border
                && border.Child is TextBlock intermediateCell
                && intermediateCell.Text != " ")
            {
                return false;
            }

            return true;
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

        private bool IsBlackPiece(TextBlock targetCell)
        {
            return targetCell.Foreground == Brushes.Black;
        }
    }
}
