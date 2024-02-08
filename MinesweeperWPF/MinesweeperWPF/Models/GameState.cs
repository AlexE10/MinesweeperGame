using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.Models
{
    public class GameState
    {
        public Board Board { get; }
        public bool GameOver { get; }
        public bool GameWon { get; }

        public int AvailableFlags { get; }

        public GameState(Board board, int availableFlags, bool gameOver, bool gameWon)
        {
            Board = board;
            GameOver = gameOver;
            GameWon = gameWon;
            AvailableFlags = availableFlags;
        }
    }

}
