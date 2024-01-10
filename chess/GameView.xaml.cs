using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chess;

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

        private ChessBoard board = new ChessBoard();

        private Dictionary<int, int> blackRows  = new Dictionary<int, int>()
        {
            { 0, 1 },
            { 1, 2 },
            { 2, 3 },
            { 3, 4 },
            { 4, 5 },
            { 5, 6 },
            { 6, 7 },
            { 7, 8 },
        };

        private Dictionary<int, string> blackColumns = new Dictionary<int, string>()
        {
            { 1, "h" },
            { 2, "g" },
            { 3, "f" },
            { 4, "e" },
            { 5, "d" },
            { 6, "c" },
            { 7, "b" },
            { 8, "a" },
        };

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

            if (board.IsEndGame)
            {
                EndGame();
            }
        }

        private bool IsValidMove(int fromRow, int fromColumn, int toRow, int toColumn, TextBlock targetCell)
        {
            string fromCell = blackColumns[fromColumn] + blackRows[fromRow];
            string toCell = blackColumns[toColumn] + blackRows[toRow];
            
            if (board[fromCell] is null)
            {
                return false;
            }

            Move move = new Move(fromCell, toCell);
 
            if (board.IsValidMove(move))
            {
                board.Move(move);
                return true;
            }
            return false;
        }

        private void EndGame()
        {
            throw new Exception("победили");
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

        private Border GetCell(int row, int column)
        {
            return (Border)ChessGrid.Children.Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);
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

        //Выбор новой фигуры для пешки
        private void PromotePawn(TextBlock targetCell)
        {
            
        }
    }
}
