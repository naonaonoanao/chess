using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwp
{
    public class ChessGame
    {
        private GameView gameView; // Ссылка на ваш контрол для обновления интерфейса


        public ChessGame(GameView gameView)
        {
            this.gameView = gameView;

        }

        public void MakeMove()
        {
            // Реализуйте логику ходов с использованием вашей обученной модели
            // Например, вызовите методы модели для предсказания следующего хода
            // и выполните соответствующие действия на шахматной доске
            // Обновите интерфейс после каждого хода
            // gameView.UpdateBoard();
        }
    }
}
