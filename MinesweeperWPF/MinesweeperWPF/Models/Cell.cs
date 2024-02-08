using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.Models
{
    public class Cell
    {
        public bool IsMine { get; }
        public bool IsRevealed { get; }
        public bool IsFlagged { get; }
        public int AdjacentMines { get; }

        public Cell(bool isMine, bool isRevealed, bool isFlagged, int adjacentMines)
        {
            IsMine = isMine;
            IsRevealed = isRevealed;
            IsFlagged = isFlagged;
            AdjacentMines = adjacentMines;
        }
    }
}
