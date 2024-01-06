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
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        public GameView()
        {
            InitializeComponent();
            InitializeChessBoard();
            
        }

        private void InitializeChessBoard()
        {
            // Создаем шахматные фигуры
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    chessPieces.Add(new ChessPiece
                    {
                        Row = row,
                        Col = col,
                        Color = (row + col) % 2 == 0 ? Brushes.White : Brushes.Black,
                        PieceType = ChessPieceType.None // В этом примере все ячейки начинаются пустыми
                    });
                }
            }

            // Отображаем фигуры на доске
            foreach (var piece in chessPieces)
            {
                var cell = new Border
                {
                    Style = (Style)FindResource("ChessCell"),
                    DataContext = piece,
                    AllowDrop = true
                };
                cell.Drop += ChessCell_Drop;

                Grid.SetRow(cell, piece.Row);
                Grid.SetColumn(cell, piece.Col);

                ChessGrid.Children.Add(cell);
            }
        }

        private void ChessCell_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ChessPiece)))
            {
                var droppedPiece = (ChessPiece)e.Data.GetData(typeof(ChessPiece));
                var dropTarget = (Border)sender;

                // Перемещаем фигуру на новую позицию
                droppedPiece.Row = Grid.GetRow(dropTarget);
                droppedPiece.Col = Grid.GetColumn(dropTarget);

                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Запуск обработчика событий для фигур
            foreach (var piece in chessPieces)
            {
                var cell = (Border)ChessGrid.Children
                    .Cast<UIElement>()
                    .First(child => Grid.GetRow(child) == piece.Row && Grid.GetColumn(child) == piece.Col);

                cell.MouseMove += ChessPiece_MouseMove;
                cell.MouseDown += ChessPiece_MouseDown;
            }
        }

        private void ChessPiece_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var piece = ((Border)sender).DataContext as ChessPiece;

            if (piece != null)
            {
                // Начало перетаскивания фигуры
                DragDrop.DoDragDrop((DependencyObject)sender, piece, DragDropEffects.Move);
            }
        }

        private void ChessPiece_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var piece = ((Border)sender).DataContext as ChessPiece;

                if (piece != null)
                {
                    // Начало перетаскивания фигуры
                    DragDrop.DoDragDrop((DependencyObject)sender, piece, DragDropEffects.Move);
                }
            }
        }
    }

    public class ChessPiece
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Brush Color { get; set; }
        public ChessPieceType PieceType { get; set; }
    }

    public enum ChessPieceType
    {
        None,
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
}