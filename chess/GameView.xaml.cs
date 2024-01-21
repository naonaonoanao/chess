using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Chess;

namespace uwp
{
    public partial class GameView : UserControl
    {
        public event EventHandler RequestChangeContent;
        public bool isSecondPlayerMove = false;
        public bool isFirstMove = false;
        private string promotionType = "";
        public bool isGameStarted = false;

        private PromotionType userPromotion = PromotionType.ToQueen;
        private PromotionType? neuroPromotion;

        public GameView()
        {
            MakeMove("clear-board", "");
            InitializeComponent();
            board.OnPromotePawn += HandlePromotePawn;
        }

        private void HandlePromotePawn(object sender, PromotionEventArgs e)
        {
            Debug.WriteLine("PROMOTION!!!!");
            if (neuroPromotion != null)
            {
                e.PromotionResult = (PromotionType)neuroPromotion;
                promotionType = promotionDict[(PromotionType)neuroPromotion];
            }
            else
            {
                e.PromotionResult = userPromotion;
                promotionType = promotionDict[userPromotion];
            } 
        }

        public void UpdateColor()
        {

            if (isSecondPlayerMove) // если игрок черный
            {
                ChessGrid.Resources["NeuroColor"] = new SolidColorBrush(Colors.Black);
                ChessGrid.Resources["PlayerColor"] = new SolidColorBrush(Colors.White);

                if (!isFirstMove) // если не сделан первый ход в игре
                {
                    
                    ((GetCell(1, 4).Child as TextBlock).Text, (GetCell(1, 5).Child as TextBlock).Text) = ((GetCell(1, 5).Child as TextBlock).Text, (GetCell(1, 4).Child as TextBlock).Text);
                    ((GetCell(8, 4).Child as TextBlock).Text, (GetCell(8, 5).Child as TextBlock).Text) = ((GetCell(8, 5).Child as TextBlock).Text, (GetCell(8, 4).Child as TextBlock).Text);
                    
                    var worker = new BackgroundWorker(); // против лага

                    worker.DoWork += (s, arg) =>
                    {
                        var neuroMove = MakeMove("make-neuro-move", "");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ApplyNeuroMove(neuroMove);
                        });
                    };

                    worker.RunWorkerCompleted += (s, arg) =>
                    {
                        showMovesHistory();
                    };

                    worker.RunWorkerAsync();
                    isFirstMove = true;
                }
            }
            else
            {
                ChessGrid.Resources["NeuroColor"] = new SolidColorBrush(Colors.White);
                ChessGrid.Resources["PlayerColor"] = new SolidColorBrush(Colors.Black);
            }
        }

        private const string ServerUrl = "http://localhost:8000/";

        private TextBlock? selectedCell;
        private Style? selectedPrevStyle;

        private ChessBoard board = new ChessBoard();

        private Dictionary<PromotionType, string> promotionDict = new Dictionary<PromotionType, string>()
        {
            { PromotionType.ToBishop, "b" },
            { PromotionType.ToQueen, "q" },
            { PromotionType.ToKnight, "n" },
            { PromotionType.ToRook, "r" },
        };

        private Dictionary<string, string> piecesDict = new Dictionary<string, string>()
        {
            { "b", "♝" },
            { "q", "♛" },
            { "n", "♞" },
            { "r", "♜" },
        };

        private Dictionary<int, int> blackRows  = new Dictionary<int, int>()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 },
            { 6, 6 },
            { 7, 7 },
            { 8, 8 },
        };

        private Dictionary<int, int> blackRowsReverse = new Dictionary<int, int>()
        {
            { 1, 8 },
            { 2, 7 },
            { 3, 6 },
            { 4, 5 },
            { 5, 4 },
            { 6, 3 },
            { 7, 2 },
            { 8, 1 },
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

        private Dictionary<int, string> blackColumnsReverse = new Dictionary<int, string>()
        {
            { 1, "a" },
            { 2, "b" },
            { 3, "c" },
            { 4, "d" },
            { 5, "e" },
            { 6, "f" },
            { 7, "g" },
            { 8, "h" },
        };

        private Dictionary<int, int> ReversedDictionaryRows(Dictionary<int, int> original)
        {
            return original.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private Dictionary<string, int> ReversedDictionaryColumns(Dictionary<int, string> original)
        {
            return original.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private Dictionary<string, PromotionType> ReversedPromDict(Dictionary<PromotionType, string> original)
        {
            return original.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }


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
                    TextBlock targetCell = (TextBlock)element;
                    
                    MovePiece(targetCell);
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
            var worker = new BackgroundWorker();
            int fromRow = Grid.GetRow((UIElement)selectedCell.Parent);
            int fromColumn = Grid.GetColumn((UIElement)selectedCell.Parent);
            int toRow = Grid.GetRow((UIElement)targetCell.Parent);
            int toColumn = Grid.GetColumn((UIElement)targetCell.Parent);

            if (IsValidMove(fromRow, fromColumn, toRow, toColumn, targetCell))
            {
                isGameStarted = true;
                isFirstMove = true;
                SwapPieces(selectedCell, targetCell);
                selectedCell = null;

                // Добавляем выделение ячеек
                HighlightMovedCellsPers(fromRow, fromColumn, toRow, toColumn);

                worker.DoWork += (s, arg) =>
                {
                    // Применяем ход к доске, используя ваши существующие методы
                    string userMove;
                    if (isSecondPlayerMove)
                    {
                        userMove = $"{blackColumns[fromColumn]}{blackRows[fromRow]}{blackColumns[toColumn]}{blackRows[toRow]}";
                    }
                    else
                    {
                        userMove = $"{blackColumnsReverse[fromColumn]}{blackRowsReverse[fromRow]}{blackColumnsReverse[toColumn]}{blackRowsReverse[toRow]}";
                    }

                    Debug.WriteLine(userMove);

                    if (promotionType != "")
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            targetCell.Text = piecesDict[promotionType];
                        });
                    }
                    
                    MakeMove($"make-user-move/?move={userMove+promotionType}", "");
                    promotionType = "";

                    var neuroMove = MakeMove("make-neuro-move", "");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ApplyNeuroMove(neuroMove);
                    });
                };

                worker.RunWorkerCompleted += (s, arg) =>
                {
                    showMovesHistory();

                    if (board.IsEndGame)
                    {
                        EndGame();
                    }
                };

                worker.RunWorkerAsync();

                showMovesHistory();

                if (board.IsEndGame)
                {
                    EndGame();
                }
            } 
            else
            {
                selectedCell = null;
            }
        }

        private TextBlock? selectedCellBeforePers;
        private Style? selectedPrevStyleBeforePers;
        private TextBlock? selectedCellAfterPers;
        private Style? selectedPrevStyleAfterPers;

        private TextBlock? selectedCellBeforeNeuro;
        private Style? selectedPrevStyleBeforeNeuro;
        private TextBlock? selectedCellAfterNeuro;
        private Style? selectedPrevStyleAfterNeuro;


        private void ReturnWithoutHightlightMove()
        {
            if (selectedCellBeforePers != null && selectedCellBeforePers.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCellBeforePers.Parent;
                selectedBorder.Style = selectedPrevStyleBeforePers; 
            }

            if (selectedCellAfterPers != null && selectedCellAfterPers.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCellAfterPers.Parent;
                selectedBorder.Style = selectedPrevStyleAfterPers;
            }

            if (selectedCellBeforeNeuro != null && selectedCellBeforeNeuro.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCellBeforeNeuro.Parent;
                selectedBorder.Style = selectedPrevStyleBeforeNeuro;
            }

            if (selectedCellAfterNeuro != null && selectedCellAfterNeuro.Parent is Border)
            {
                Border selectedBorder = (Border)selectedCellAfterNeuro.Parent;
                selectedBorder.Style = selectedPrevStyleAfterNeuro;
            }
        }

        
        // метод для выделения ячеек человека
        private void HighlightMovedCellsPers(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            {
                // Сбрасываем предыдущее выделение
                ReturnWithoutHightlightMove();

                // Выделяем ячейку, откуда сделан ход
                if (GetCell(fromRow, fromColumn).Child is TextBlock fromCellTextBlock)
                {
                    // Запоминаем текущие выделенные ячейки для следующего сброса
                    selectedCellBeforePers = fromCellTextBlock;
                    Border selectedBorder = (Border)selectedCellBeforePers.Parent;
                    selectedPrevStyleBeforePers = selectedBorder.Style;

                    Border fromCellBorder = (Border)fromCellTextBlock.Parent;
                    fromCellBorder.Style = ChessGrid.Resources["ChessMoveCellPers"] as Style;
                }

                // Выделяем ячейку, куда сделан ход
                if (GetCell(toRow, toColumn).Child is TextBlock toCellTextBlock)
                {
                    selectedCellAfterPers = toCellTextBlock;
                    Border selectedBorder = (Border)selectedCellAfterPers.Parent;
                    selectedPrevStyleAfterPers = selectedBorder.Style;

                    Border toCellBorder = (Border)toCellTextBlock.Parent;
                    toCellBorder.Style = ChessGrid.Resources["ChessMoveCellPers"] as Style;

                }
            }
        }
        
        // метод для выделения ячеек нейронки
        private void HighlightMovedCellsNeuro(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            {
                // Сбрасываем предыдущее выделение
                ReturnWithoutHightlightMove();

                // Выделяем ячейку, откуда сделан ход
                if (GetCell(fromRow, fromColumn).Child is TextBlock fromCellTextBlock)
                {
                    // Запоминаем текущие выделенные ячейки для следующего сброса
                    selectedCellBeforeNeuro = fromCellTextBlock;
                    Border selectedBorder = (Border)selectedCellBeforeNeuro.Parent;
                    selectedPrevStyleBeforeNeuro = selectedBorder.Style;

                    Border fromCellBorder = (Border)fromCellTextBlock.Parent;
                    fromCellBorder.Style = ChessGrid.Resources["ChessMoveCellNeuro"] as Style;

                }

                // Выделяем ячейку, куда сделан ход
                if (GetCell(toRow, toColumn).Child is TextBlock toCellTextBlock)
                {
                    selectedCellAfterNeuro = toCellTextBlock;
                    Border selectedBorder = (Border)selectedCellAfterNeuro.Parent;
                    selectedPrevStyleAfterNeuro = selectedBorder.Style;

                    Border toCellBorder = (Border)toCellTextBlock.Parent;
                    toCellBorder.Style = ChessGrid.Resources["ChessMoveCellNeuro"] as Style;
                }
            }
        }

        private void showMovesHistory()
        {
            history.Children.Clear();
            int i = 1;

            for (int index = 0; index < board.ExecutedMoves.Count; index += 2)
            {
                var move1 = board.ExecutedMoves[index];
                var move2 = (index + 1 < board.ExecutedMoves.Count) ? board.ExecutedMoves[index + 1] : null;

                string fromCellMove1;
                string toCellMove1;
                string fromCellMove2 = string.Empty;
                string toCellMove2 = string.Empty;

                if (isSecondPlayerMove)
                {
                    fromCellMove1 = blackColumns[move1.OriginalPosition.X + 1] + blackRowsReverse[move1.OriginalPosition.Y + 1];
                    toCellMove1 = blackColumns[move1.NewPosition.X + 1] + blackRowsReverse[move1.NewPosition.Y + 1];

                    if (move2 != null)
                    {
                        fromCellMove2 = blackColumns[move2.OriginalPosition.X + 1] + blackRowsReverse[move2.OriginalPosition.Y + 1];
                        toCellMove2 = blackColumns[move2.NewPosition.X + 1] + blackRowsReverse[move2.NewPosition.Y + 1];
                    }
                }
                else
                {
                    fromCellMove1 = blackColumnsReverse[move1.OriginalPosition.X + 1] + blackRows[move1.OriginalPosition.Y + 1];
                    toCellMove1 = blackColumnsReverse[move1.NewPosition.X + 1] + blackRows[move1.NewPosition.Y + 1];

                    if (move2 != null)
                    {
                        fromCellMove2 = blackColumnsReverse[move2.OriginalPosition.X + 1] + blackRows[move2.OriginalPosition.Y + 1];
                        toCellMove2 = blackColumnsReverse[move2.NewPosition.X + 1] + blackRows[move2.NewPosition.Y + 1];
                    }
                }

                // Создаем новый Border для каждой пары ходов
                Border moveBorder = new Border
                {
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(5)
                };

                // Создаем новый TextBlock для хода
                TextBlock moveTextBlock = new TextBlock
                {
                    Text = $"{i}. {fromCellMove1} - {toCellMove1}",
                    Foreground = Brushes.White,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Если второй ход в паре выполнен, добавляем разделитель и второй ход
                if (move2 != null)
                {
                    moveTextBlock.Text += $" | {fromCellMove2} - {toCellMove2}";
                }

                // Устанавливаем TextBlock как Child для Border
                moveBorder.Child = moveTextBlock;

                // Добавляем Border в StackPanel
                history.Children.Add(moveBorder);

                i++;
            }
        }



        private string MakeMove(string endpoint, string move)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{ServerUrl}{endpoint}";
                var content = new StringContent(move, Encoding.UTF8, "application/json");

                var response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    // Обработка ошибок
                    Debug.WriteLine($"Error: {response.StatusCode}");
                    return string.Empty;
                }
            }
        }

        private void ApplyNeuroMove(string neuroMove)
        {
            isGameStarted = true;

            Dictionary<int, int> reversedBlackRows = ReversedDictionaryRows(blackRows);
            Dictionary<string, int> reversedBlackColumns = ReversedDictionaryColumns(blackColumns);
            Dictionary<int, int> reversedBlackRowsReverse = ReversedDictionaryRows(blackRowsReverse);
            Dictionary<string, int> reversedBlackColumnsReverse = ReversedDictionaryColumns(blackColumnsReverse);
            Dictionary<string, PromotionType> reversedPromotionDict = ReversedPromDict(promotionDict);

            // Получаем значение хода нейросети
            JsonDocument jsonDocument = JsonDocument.Parse(neuroMove);
            jsonDocument.RootElement.TryGetProperty("neuro_move", out JsonElement neuroMoveElement);

            string neuroMoveValue = neuroMoveElement.GetString();

            if (neuroMoveValue.Length == 5)
            {
                promotionType = neuroMoveValue[4].ToString();
                neuroPromotion = reversedPromotionDict[promotionType];

                neuroMoveValue = neuroMoveValue.Substring(0, 4);
            }
                // Применяем ход к доске
            int fromRow;
            int fromColumn;
            int toRow;
            int toColumn;

            // Применяем ход к доске, используя ваши существующие методы
            string fromCell;
            string toCell;
            if (isSecondPlayerMove)
            {
                fromRow = reversedBlackRows[int.Parse(neuroMoveValue[1].ToString())];
                fromColumn = reversedBlackColumns[neuroMoveValue[0].ToString()];
                toRow = reversedBlackRows[int.Parse(neuroMoveValue[3].ToString())];
                toColumn = reversedBlackColumns[neuroMoveValue[2].ToString()];

                fromCell = blackColumns[fromColumn] + blackRows[fromRow];
                toCell = blackColumns[toColumn] + blackRows[toRow];
            }
            else
            {
                fromRow = reversedBlackRowsReverse[int.Parse(neuroMoveValue[1].ToString())];
                fromColumn = reversedBlackColumnsReverse[neuroMoveValue[0].ToString()];
                toRow = reversedBlackRowsReverse[int.Parse(neuroMoveValue[3].ToString())];
                toColumn = reversedBlackColumnsReverse[neuroMoveValue[2].ToString()];

                fromCell = blackColumnsReverse[fromColumn] + blackRowsReverse[fromRow];
                toCell = blackColumnsReverse[toColumn] + blackRowsReverse[toRow];
            }

            Move move = new Move(fromCell, toCell);
            board.Move(move);

            // Вызываем метод выделения ячеек при ходе нейронной сети
            HighlightMovedCellsNeuro(fromRow, fromColumn, toRow, toColumn);

            if (board.IsEndGame)
            {
                EndGame();
            }

            SwapPieces(GetCell(fromRow, fromColumn).Child as TextBlock, GetCell(toRow, toColumn).Child as TextBlock);

            if (promotionType != "")
            {
                (GetCell(toRow, toColumn).Child as TextBlock).Text = piecesDict[promotionType];
            }

            neuroPromotion = null;
            promotionType = "";
        }

        private Border GetCell(int row, int column)
        {
            return (Border)ChessGrid.Children.Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);
        }


        private bool IsValidMove(int fromRow, int fromColumn, int toRow, int toColumn, TextBlock targetCell)
        {
            string fromCell;
            string toCell;

            if (isSecondPlayerMove)
            {
                fromCell = blackColumns[fromColumn] + blackRows[fromRow];
                toCell = blackColumns[toColumn] + blackRows[toRow];
            }
            else
            {
                fromCell = blackColumnsReverse[fromColumn] + blackRowsReverse[fromRow];
                toCell = blackColumnsReverse[toColumn] + blackRowsReverse[toRow];
            }

            if (selectedCell.Foreground is SolidColorBrush foregroundBrush)
            {
                if (foregroundBrush.Color == Colors.Black && !isSecondPlayerMove)
                {
                    return false;
                }
                else if (foregroundBrush.Color == Colors.White && isSecondPlayerMove)
                {
                    return false;
                }
            }

            if (board[fromCell] is null)
            {
                return false;
            }
            
            Move move = new Move(fromCell, toCell);

            Debug.WriteLine(move.ToString());
            Debug.WriteLine(board.IsValidMove(move));
            if (board.IsValidMove(move))
            {
                board.Move(move);

                Console.WriteLine(board.ToAscii());
                Console.WriteLine(board.ToPgn());

                return true;
            }
            return false;
        }

        private void EndGame()
        {
            throw new Exception("победили");
        }

        //Замена фигур
        private void SwapPieces(TextBlock sourceCell, TextBlock targetCell)
        {
            var oldForeground = sourceCell.Foreground;
            sourceCell.Foreground = targetCell.Foreground;
            targetCell.Foreground = oldForeground;

            targetCell.Text = sourceCell.Text;
            sourceCell.Text = " ";
        }

        private bool isBorderVisible = false;

        private void ToggleBorderButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBorderVisible)
            {
                // Если бордер видим, скрываем его
                HideBorder();
            }
            else
            {
                // Если бордер скрыт, показываем его
                ShowBorder();
            }

            // Инвертируем состояние видимости
            isBorderVisible = !isBorderVisible;
        }

        private void ShowBorder()
        {
            // Создание анимации изменения высоты
            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = 0;
            heightAnimation.To = 541;
            heightAnimation.Duration = TimeSpan.FromSeconds(0.5);

            // Применение анимации к высоте бордера
            animatedBorder.BeginAnimation(Border.HeightProperty, heightAnimation);
        }

        private void HideBorder()
        {
            // Создание анимации изменения высоты
            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = 541;
            heightAnimation.To = 0;
            heightAnimation.Duration = TimeSpan.FromSeconds(0.5);

            // Применение анимации к высоте бордера
            animatedBorder.BeginAnimation(Border.HeightProperty, heightAnimation);
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            string windowName = "menuWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);
        }
    }
}
