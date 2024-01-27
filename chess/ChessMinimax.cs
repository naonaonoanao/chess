using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwp
{
    public class ChessMinimax
    {
        private Dictionary<string, int> pieceCost = new Dictionary<string, int>()
        {
            { "wp", 1 },
            { "bp", -1 },
            { "wn", 3 },
            { "bn", -3 },
            { "wb", 4 },
            { "bb", -4 },
            { "wr", 5 },
            { "br", -5 },
            { "wq", 9 },
            { "bq", -9 },
            { "wk", 10 },
            { "bk", -10 }
        };

        public ChessMinimax()
        {
        }

        private int EvaluatePositionByPieceCost(ChessBoard board)
        {
            int boardCost = 0;

            foreach (char i in "abcdefgh")
            {
                for (short j = 1; j <= 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null)
                    {
                        boardCost += -pieceCost[piece.ToString()];
                    }
                }
            }

            return boardCost;
        }

        public Move GetBestMove(string fen, bool isBlack)
        {
            ChessBoard board = new ChessBoard();
            ChessBoard.TryLoadFromFen(fen, out board);

            int maxCost = -99999;
            Move bestMove = null;
            int sideKoef = isBlack ? -1 : 1;

            foreach (Move ourMove in board.Moves())
            {
                ChessBoard boardOurMove = new ChessBoard();
                ChessBoard.TryLoadFromFen(board.ToFen(), out boardOurMove);

                boardOurMove.Move(ourMove);

                int ourMoveCost = sideKoef * EvaluatePositionByPieceCost(boardOurMove);
                int minCost = 99999;

                foreach (Move enemyMove in boardOurMove.Moves())
                {
                    ChessBoard boardEnemyMove = new ChessBoard();
                    ChessBoard.TryLoadFromFen(boardOurMove.ToFen(), out boardEnemyMove);

                    boardEnemyMove.Move(enemyMove);

                    int enemyMoveCost = sideKoef * EvaluatePositionByPieceCost(boardEnemyMove);

                    minCost = Math.Min(enemyMoveCost, minCost);
                }

                int moveCost = minCost + ourMoveCost;

                if (moveCost > maxCost) 
                {
                    maxCost = moveCost;
                    bestMove = ourMove;
                }
            }

            return bestMove;
        }
    }
}
