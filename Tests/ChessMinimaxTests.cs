using uwp;
using Chess;

namespace Tests
{
    [TestFixture]
    public class ChessMinimaxTests
    {
        [Test]
        public void GetBestMove_ReturnsValidMove()
        {
            string initialFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            bool isBlack = false;
            ChessMinimax minimax = new ChessMinimax();

            Move bestMove = minimax.GetBestMove(initialFen, isBlack);

            Assert.IsTrue(bestMove != null);
        }
    }
}
