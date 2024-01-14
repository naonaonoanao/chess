using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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

        private const string ServerUrl = "http://localhost:8000/";

        private TextBlock? selectedCell;
        private Style? selectedPrevStyle;
        private bool IsWhiteTurn = true;

        private ChessBoard board = new ChessBoard();

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

        private Dictionary<int, int> ReversedDictionaryRows(Dictionary<int, int> original)
        {
            return original.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private Dictionary<string, int> ReversedDictionaryColumns(Dictionary<int, string> original)
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

                string userMove = $"{blackColumns[fromColumn]}{blackRows[fromRow]}{blackColumns[toColumn]}{blackRows[toRow]}";
                Debug.WriteLine(userMove);
                Task<string> userMoveTask = Task.Run(() => MakeMove($"make-user-move/?move={userMove}", ""));
                userMoveTask.Wait(); // Блокируем основной поток до завершения задачи
                string userMoveResponse = userMoveTask.Result;

                // Отправка запроса на получение хода от нейросети
                Task<string> neuroMoveTask = Task.Run(() => MakeMove("make-neuro-move", ""));
                neuroMoveTask.Wait(); // Блокируем основной поток до завершения задачи
                string neuroMoveResponse = neuroMoveTask.Result;

                ApplyNeuroMove(neuroMoveResponse);
            }

            selectedCell = null;

            if (board.IsEndGame)
            {
                EndGame();
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
            Dictionary<int, int> reversedBlackRows = ReversedDictionaryRows(blackRows);
            Dictionary<string, int> reversedBlackColumns = ReversedDictionaryColumns(blackColumns);

            // Получаем значение хода нейросети
            JsonDocument jsonDocument = JsonDocument.Parse(neuroMove);
            jsonDocument.RootElement.TryGetProperty("neuro_move", out JsonElement neuroMoveElement);

            string neuroMoveValue = neuroMoveElement.GetString();

            // Применяем ход к доске
            // Например, парсинг хода и применение к доске
            int fromRow = reversedBlackRows[int.Parse(neuroMoveValue[1].ToString())];
            int fromColumn = reversedBlackColumns[neuroMoveValue[0].ToString()];
            int toRow = reversedBlackRows[int.Parse(neuroMoveValue[3].ToString())];
            int toColumn = reversedBlackColumns[neuroMoveValue[2].ToString()];

            // Применяем ход к доске, используя ваши существующие методы
            // Например:
            string fromCell = blackColumns[fromColumn] + blackRows[fromRow];
            string toCell = blackColumns[toColumn] + blackRows[toRow];

            Move move = new Move(fromCell, toCell);
            board.Move(move);

            if (board.IsEndGame)
            {
                EndGame();
            }

            SwapPieces(GetCell(fromRow, fromColumn).Child as TextBlock, GetCell(toRow, toColumn).Child as TextBlock);
        }

        private Border GetCell(int row, int column)
        {
            return (Border)ChessGrid.Children.Cast<UIElement>()
                .FirstOrDefault(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);
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

            Debug.WriteLine(move.ToString());

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

        //Выбор новой фигуры для пешки
        private void PromotePawn(TextBlock targetCell)
        {
            
        }
    }
}
