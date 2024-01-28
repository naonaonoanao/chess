using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using chess;
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
        UserRepository userRepository = new UserRepository();

        private PromotionType? userPromotion;
        private PromotionType? neuroPromotion;
        public string login;

        public bool isMinimax;
        ChessMinimax minimax = new ChessMinimax();

        public GameView(bool _isMinimax = true)
        {
            isMinimax = _isMinimax;
            if (!isMinimax)
            {
            MakeMove("clear-board", "");
            }
            InitializeComponent();
            RedrawBoard();
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
                var dialog = new PromoteDialog();
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    userPromotion = dialog.getUserPromotion;
                }

                e.PromotionResult = (PromotionType)userPromotion;
                promotionType = promotionDict[(PromotionType)userPromotion];
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
                        if (isMinimax)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ApplyMinimaxMove();
                            });
                        }
                        else
                        {
                            var neuroMove = MakeMove("make-neuro-move", "");

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ApplyNeuroMove(neuroMove);
                            });
                        }
                    };

                    worker.RunWorkerCompleted += (s, arg) =>
                    {
                        ShowMovesHistory();
                        ShowCapturedFigures();
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

        private const string ServerUrl = "http://89.23.106.97:6565/";

        private TextBlock? selectedCell;

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
                int fromRow = Grid.GetRow((UIElement)selectedCell.Parent);
                int fromColumn = Grid.GetColumn((UIElement)selectedCell.Parent);
                string fromCell;
                if (fromRow > 9 || fromRow == 0 || fromColumn == 0 || fromColumn > 9)
                {
                    return;
                }
                Border selectedBorder = (Border)selectedCell.Parent;

                if (selectedBorder.Style != ChessGrid.Resources["ChessMoveCellPers"] as Style)
                {
                    selectedBorder.Style = ChessGrid.Resources["ChessCellSelectedMove"] as Style;

                }
                else
                {
                    selectedBorder.Style = ChessGrid.Resources["ChessCellSelectedMoveLast"] as Style;
                }
                if (isSecondPlayerMove)
                {
                    fromCell = blackColumns[fromColumn] + blackRows[fromRow];
                }
                else
                {
                    fromCell = blackColumnsReverse[fromColumn] + blackRowsReverse[fromRow];
                }

                Move[] moves;
                try
                {
                    moves = board.Moves(new Position(fromCell));
                }
                catch (Exception e) 
                {
                    return;
                }

                foreach (Move move in moves)
                {
                    int y;
                    int x;

                    if (isSecondPlayerMove)
                    {
                        y = move.NewPosition.Y + 1;
                        x = 8 - move.NewPosition.X;
                    }
                    else
                    {
                        y = 8 - move.NewPosition.Y;
                        x = move.NewPosition.X + 1;
                    }

                    var cell = GetCell(y, x);
                    if (cell.Style != ChessGrid.Resources["ChessMoveCellPers"] as Style)
                    {
                        cell.Style = ChessGrid.Resources["ChessCellSelectedMove"] as Style;
                    }
                    else
                    {
                        cell.Style = ChessGrid.Resources["ChessCellSelectedMoveLast"] as Style;
                    }
                }
            }
        }

        private void HighlightKing(Position king)
        {
            int y;
            int x;

            if (isSecondPlayerMove)
            {
                y = king.Y + 1;
                x = 8 - king.X;
            }
            else
            {
                y = 8 - king.Y;
                x = king.X + 1;
            }

            var cell = GetCell(y, x);

            cell.Style = ChessGrid.Resources["ChessCellChecked"] as Style;
        }

        private void UnhighlightKing()
        {
            foreach (UIElement element in ChessGrid.Children)
            {
                if (element is Border border && border.Style == ChessGrid.Resources["ChessCellChecked"] as Style)
                {
                    int row = Grid.GetRow(border);
                    int column = Grid.GetColumn(border);

                    if ((row + column) % 2 == 0)
                    {
                        border.Style = ChessGrid.Resources["ChessCellWhite"] as Style;
                    }
                    else
                    {
                        border.Style = ChessGrid.Resources["ChessCellBlack"] as Style;
                    }
                }
            }
        }

        private void ReturnPrevSelectedStyle()
        {
            foreach (UIElement element in ChessGrid.Children)
            {
                if (element is Border border && border.Style == ChessGrid.Resources["ChessCellSelectedMove"] as Style)
                {
                    int row = Grid.GetRow(border);
                    int column = Grid.GetColumn(border);

                    if ((row + column) % 2 == 0)
                    {
                        border.Style = ChessGrid.Resources["ChessCellWhite"] as Style;
                    }
                    else
                    {
                        border.Style = ChessGrid.Resources["ChessCellBlack"] as Style;
                    }
                }
                else if (element is Border border2 && border2.Style == ChessGrid.Resources["ChessCellSelectedMoveLast"] as Style)
                {
                    border2.Style = ChessGrid.Resources["ChessMoveCellPers"] as Style;
                }
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
                //SwapPieces(selectedCell, targetCell);
                Debug.Write(board.ToAscii());

                RedrawBoard();

                selectedCell = null;

                // Добавляем выделение ячеек
                HighlightMovedCellsPers(fromRow, fromColumn, toRow, toColumn);

                worker.DoWork += (s, arg) =>
                {
                    // Применяем ход к доске
                    if (isMinimax)
                    {
                        if (promotionType != "")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                targetCell.Text = piecesDict[promotionType];
                            });
                        }
                        promotionType = "";

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ApplyMinimaxMove();
                        }
                        );
                    }
                    else
                    {
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

                        MakeMove($"make-user-move/?move={userMove + promotionType}", "");
                        promotionType = "";

                        var neuroMove = MakeMove("make-neuro-move", "");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ApplyNeuroMove(neuroMove);
                        });
                    }
                };

                worker.RunWorkerCompleted += (s, arg) =>
                {
                    ShowMovesHistory();
                    ShowCapturedFigures();

                    if (board.BlackKingChecked && kingChecked != "b")
                    {
                        kingChecked = "b";
                        HighlightKing(board.BlackKing);
                    }
                    else if (board.WhiteKingChecked && kingChecked != "w")
                    {
                        kingChecked = "w";
                        HighlightKing(board.WhiteKing);
                    }
                    else if (kingChecked != "")
                    {
                        kingChecked = "";
                        UnhighlightKing();
                    }

                    if (board.IsEndGame)
                    {
                        EndGame();
                    }
                };

                worker.RunWorkerAsync();

                if (board.BlackKingChecked && kingChecked != "b")
                {
                    kingChecked = "b";
                    HighlightKing(board.BlackKing);
                }
                else if (board.WhiteKingChecked && kingChecked != "w")
                {
                    kingChecked = "w";
                    HighlightKing(board.WhiteKing);
                }
                else if (kingChecked != "")
                {
                    kingChecked = "";
                    UnhighlightKing();
                }

                ShowMovesHistory();
                ShowCapturedFigures();

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

        string kingChecked = "";

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

        
        // Метод для выделения ячеек человека
        private void HighlightMovedCellsPers(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            {
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
        
        // Метод для выделения ячеек нейронки
        private void HighlightMovedCellsNeuro(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            {
                ReturnWithoutHightlightMove();

                // Выделяем ячейку, откуда сделан ход
                if (GetCell(fromRow, fromColumn).Child is TextBlock fromCellTextBlock)
                {
                    // Запоминаем текущие выделенные ячейки для следующего сброса
                    selectedCellBeforeNeuro = fromCellTextBlock;
                    Border selectedBorder = (Border)selectedCellBeforeNeuro.Parent;
                    selectedPrevStyleBeforeNeuro = selectedBorder.Style;

                    Border fromCellBorder = (Border)fromCellTextBlock.Parent;
                    fromCellBorder.Style = ChessGrid.Resources["ChessMoveCellPers"] as Style;
                }

                // Выделяем ячейку, куда сделан ход
                if (GetCell(toRow, toColumn).Child is TextBlock toCellTextBlock)
                {
                    selectedCellAfterNeuro = toCellTextBlock;
                    Border selectedBorder = (Border)selectedCellAfterNeuro.Parent;
                    selectedPrevStyleAfterNeuro = selectedBorder.Style;

                    Border toCellBorder = (Border)toCellTextBlock.Parent;
                    toCellBorder.Style = ChessGrid.Resources["ChessMoveCellPers"] as Style;
                }
            }
        }


        // Показ статистики в левом бордере
        private void ShowStatisticHistory(User user)
        {
            Border moveBorder = new Border
            {
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 15, 0, 8),
                Padding = new Thickness(5)
            };


            TextBlock usernameTextBlock = new TextBlock
            {
                Text = $"Логин: {user.Username}",
                Foreground = Brushes.White,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock totalGamesTextBlock = new TextBlock
            {
                Text = $"Всего: {user.WinCount + user.LoseCount + user.DrawCount}",
                Foreground = Brushes.White,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock winsTextBlock = new TextBlock
            {
                Text = $"Выиграно: {user.WinCount}",
                Foreground = Brushes.White,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock losesTextBlock = new TextBlock
            {
                Text = $"Проиграно: {user.LoseCount}",
                Foreground = Brushes.White,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock drawsTextBlock = new TextBlock
            {
                Text = $"Сыграно в ничью: {user.DrawCount}",
                Foreground = Brushes.White,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };


            moveBorder.Child = usernameTextBlock;
            statistic.Children.Add(moveBorder);

            moveBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Margin = new Thickness(0, 8, 0, 8), Padding = new Thickness(5) };
            moveBorder.Child = totalGamesTextBlock;
            statistic.Children.Add(moveBorder);

            moveBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Margin = new Thickness(0, 8, 0, 8), Padding = new Thickness(5) };
            moveBorder.Child = winsTextBlock;
            statistic.Children.Add(moveBorder);

            moveBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Margin = new Thickness(0, 8, 0, 8), Padding = new Thickness(5) };
            moveBorder.Child = losesTextBlock;
            statistic.Children.Add(moveBorder);

            moveBorder = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Margin = new Thickness(0, 8, 0, 8), Padding = new Thickness(5) };
            moveBorder.Child = drawsTextBlock;
            statistic.Children.Add(moveBorder);
        }


        private void ShowMovesHistory()
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

                Border moveBorder = new Border
                {
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(5)
                };

                TextBlock moveTextBlock = new TextBlock
                {
                    Text = $"{i}. {fromCellMove1} - {toCellMove1}",
                    Foreground = Brushes.White,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (move2 != null)
                {
                    moveTextBlock.Text += $" | {fromCellMove2} - {toCellMove2}";
                }

                moveBorder.Child = moveTextBlock;

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
            if (gameResultPopup.IsOpen)
            {
                // Если попап уже открыт, выходим из метода
                return;
            }

            isGameStarted = true;

            Dictionary<int, int> reversedBlackRows = ReversedDictionaryRows(blackRows);
            Dictionary<string, int> reversedBlackColumns = ReversedDictionaryColumns(blackColumns);
            Dictionary<int, int> reversedBlackRowsReverse = ReversedDictionaryRows(blackRowsReverse);
            Dictionary<string, int> reversedBlackColumnsReverse = ReversedDictionaryColumns(blackColumnsReverse);
            Dictionary<string, PromotionType> reversedPromotionDict = ReversedPromDict(promotionDict);

            try
            {
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

                //SwapPieces(GetCell(fromRow, fromColumn).Child as TextBlock, GetCell(toRow, toColumn).Child as TextBlock);
                RedrawBoard();
                if (promotionType != "")
                {
                    (GetCell(toRow, toColumn).Child as TextBlock).Text = piecesDict[promotionType];
                }

                neuroPromotion = null;
                promotionType = "";
            }
            catch (JsonException ex)
            {
                // Обработка ошибки JSON
                Debug.WriteLine($"Error parsing neuroMove JSON: {ex.Message}");
            }
        }

        private void ApplyMinimaxMove()
        {
            if (gameResultPopup.IsOpen)
            {
                return;
            }

            isGameStarted = true;

            Dictionary<int, int> reversedBlackRows = ReversedDictionaryRows(blackRows);
            Dictionary<string, int> reversedBlackColumns = ReversedDictionaryColumns(blackColumns);
            Dictionary<int, int> reversedBlackRowsReverse = ReversedDictionaryRows(blackRowsReverse);
            Dictionary<string, int> reversedBlackColumnsReverse = ReversedDictionaryColumns(blackColumnsReverse);
            Dictionary<string, PromotionType> reversedPromotionDict = ReversedPromDict(promotionDict);

            Move minimaxMove = minimax.GetBestMove(board.ToFen(), isSecondPlayerMove);

            string neuroMoveValue = minimaxMove.OriginalPosition.ToString() + minimaxMove.NewPosition.ToString();

            if (minimaxMove.Parameter != null)
            {
                if ((minimaxMove.Parameter as MovePromotion) != null)
                {
                    promotionType = promotionDict[(minimaxMove.Parameter as MovePromotion).PromotionType];
                    neuroPromotion = reversedPromotionDict[promotionType];
                }
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

            // Вызываем метод выделения ячеек
            HighlightMovedCellsNeuro(fromRow, fromColumn, toRow, toColumn);

            if (board.IsEndGame)
            {
                EndGame();
            }

            //SwapPieces(GetCell(fromRow, fromColumn).Child as TextBlock, GetCell(toRow, toColumn).Child as TextBlock);
            RedrawBoard();
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

        private void RedrawBoard()
        {
            string letters = "abcdefgh";
            if (!isSecondPlayerMove)
            {
                for (short i = 0; i < 8; i++)
                {
                    for (short j = 1; j <= 8; j++)
                    {
                        var cell = GetCell(j, 8 - i);
                        var text = cell.Child as TextBlock;

                        var piece = board[letters[7 - i], (short)(9 - j)];
                        
                        if (piece == null)
                        {
                            text.Text = " ";
                        }
                        else
                        {
                            Debug.WriteLine($"{letters[7 - i]}{(short)(9 - j)} | {8 - i} {j} | {piece}");
                            text.Text = PieceCaptured[piece.ToString()];

                            if (piece.ToString()[0] == 'b')
                            {
                                text.Style = ChessGrid.Resources["PlayerPers"] as Style;
                            }
                            else
                            {
                                text.Style = ChessGrid.Resources["NeuroPers"] as Style;
                            }
                        }
                    }
                }
            }
            else
            {
                for (short i = 0; i < 8; i++)
                {
                    for (short j = 1; j <= 8; j++)
                    {
                        var cell = GetCell(j, 8 - i);
                        var text = cell.Child as TextBlock;

                        var piece = board[letters[i], j];
                        
                        if (piece == null)
                        {
                            text.Text = " ";
                        }
                        else
                        {
                            Debug.WriteLine($"{letters[i]}{j} | {8 - i} {j} | {piece}");
                            text.Text = PieceCaptured[piece.ToString()];

                            if (piece.ToString()[0] == 'w')
                            {
                                text.Style = ChessGrid.Resources["PlayerPers"] as Style;
                            }
                            else
                            {
                                text.Style = ChessGrid.Resources["NeuroPers"] as Style;
                            }
                        }
                    }
                }
            }
        }

            private bool IsValidMove(int fromRow, int fromColumn, int toRow, int toColumn, TextBlock targetCell)
        {
            string fromCell;
            string toCell;

            if (toColumn > 9 || toRow > 9 || toColumn == 0 || toRow == 0)
            {
                return false;
            }
            if (fromColumn > 9 || fromRow > 9 || fromColumn == 0 || fromRow == 0)
            {
                return false;
            }
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

                return true;
            }
            return false;
        }

        // Кнопка "Да" на попапе после сдаться
        private void yesGiveUpButtonClick(object sender, RoutedEventArgs e) 
        {
            if (isSecondPlayerMove)
            {
                board.Resign(PieceColor.Black);
            } else
            {
                board.Resign(PieceColor.White);
            }

            EndGame();
        }

        private Dictionary<string, string> PieceCaptured = new Dictionary<string, string>()
        {
            { "wp", "♟" },
            { "bp", "♟" },
            { "wn", "♞" },
            { "bn", "♞" },
            { "wb", "♝" },
            { "bb", "♝" },
            { "wr", "♜" },
            { "br", "♜" },
            { "wq", "♛" },
            { "bq", "♛" },
            { "wk", "♚" },
            { "bk", "♚" }
        };


        private void ShowCapturedFigures()
        {
            capturedByPerson.Children.Clear();
            capturedByNeuro.Children.Clear();


            var capturedBlackTextBlock = new TextBlock();
            var capturedWhiteTextBlock = new TextBlock();

            foreach (var piece in board.CapturedBlack)
            {
                capturedBlackTextBlock.Text += PieceCaptured[piece.ToString()] + " ";
            }

            foreach (var piece in board.CapturedWhite)
            {
                capturedWhiteTextBlock.Text += PieceCaptured[piece.ToString()] + " ";
            }

            if (isSecondPlayerMove)
            {
                capturedByNeuro.Children.Add(capturedBlackTextBlock);
                capturedByPerson.Children.Add(capturedWhiteTextBlock);
            }
            else
            {
                capturedByNeuro.Children.Add(capturedWhiteTextBlock);
                capturedByPerson.Children.Add(capturedBlackTextBlock);
            }
        }


        private void noGiveUpButtonClick(object sender, RoutedEventArgs e)
        {
            giveUpPopup.IsOpen = false;
        }


        private void EndGame()
        {
            Debug.WriteLine($"Wonside - {board.EndGame.WonSide}, player is black - {isSecondPlayerMove}");

            string mainMessage;
            string imagePath;
            string additionalMessage;

            SolidColorBrush textColor = new SolidColorBrush();

            if (board.EndGame.WonSide == null)
            {
                mainMessage = "Ничья";
                additionalMessage = "Игра не закончена, пока мы умеем дышать.";
                imagePath = "../../../image/Shiro_back.png";
                textColor = Brushes.White;
                userRepository.IncrementDraw(login);
            }
            else if ((board.EndGame.WonSide.AsChar == 'w' && !isSecondPlayerMove) || (board.EndGame.WonSide.AsChar == 'b' && isSecondPlayerMove))
            {
                mainMessage = "Победа";
                additionalMessage = "Шах и мат не значит, что вражеский король пал. " +
                                    "Это лишь объявление того, что теперь он твой.";
                imagePath = "../../../image/Sora_back.png";
                textColor = Brushes.Green;
                userRepository.IncrementWins(login);
            }
            else
            {
                mainMessage = "Поражение";
                additionalMessage = "Не существует слово «поражение» для пустых.";
                imagePath = "../../../image/Shiro_back.png";
                textColor = Brushes.Red;
                userRepository.IncrementLoses(login);
            }

            ShowPopup(mainMessage, additionalMessage, imagePath, textColor);
        }

        private void GiveUp_Click(object sender, RoutedEventArgs e)
        {
            giveUpPopup.IsOpen = true;
        }

        private void ShowPopup(string mainMessage, string additionalMessage, string imagePath, SolidColorBrush textColor)
        {
            popupMessage.Text = mainMessage;
            quotePopupMessage.Text = additionalMessage;

            popupMessage.Foreground = textColor;

            ImageBrush imageBrush = new ImageBrush(new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)));
            popupBackground.Background = imageBrush;

            gameResultPopup.IsOpen = true;
        }

        public bool isEndGame = false;

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            isEndGame = true;

            string windowName = "menuWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);

            gameResultPopup.IsOpen = false;
        }


        public bool isRestart = false;
        private void RestartButtonClick(object sender, RoutedEventArgs e)
        {
            isRestart = true;

            string windowName = "boardWindow";
            WindowEventArgs args = new WindowEventArgs(windowName);

            RequestChangeContent?.Invoke(this, args);

            gameResultPopup.IsOpen = false;
        }

        private bool isBorderVisible = false;

        private void ToggleBorderButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBorderVisible)
            {
                HideBorder();
            }
            else
            {
                ShowBorder();
            }

            isBorderVisible = !isBorderVisible;
        }

        private void ShowBorder()
        {
            // Создание анимации изменения высоты
            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = 0;
            heightAnimation.To = 541;
            heightAnimation.Duration = TimeSpan.FromSeconds(0.5);

            animatedBorder.BeginAnimation(Border.HeightProperty, heightAnimation);

            var user = userRepository.GetUserByUsername(login);
            statistic.Children.Clear();
            ShowStatisticHistory(user);
        }

        private void HideBorder()
        {
            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = 541;
            heightAnimation.To = 0;
            heightAnimation.Duration = TimeSpan.FromSeconds(0.5);

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
