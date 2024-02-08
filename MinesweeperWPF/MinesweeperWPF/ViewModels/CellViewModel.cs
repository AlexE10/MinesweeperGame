using MinesweeperWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.ViewModels
{
    public class CellViewModel
    {
        public Cell Cell { get; }
        public CellPosition Position { get; }

        public CellViewModel(Cell cell, CellPosition position)
        {
            Cell = cell;
            Position = position;
        }

        public string DisplayValue
        {
            get
            {
                if (Cell.IsRevealed)
                {
                    return Cell.IsMine ? "*" : Cell.AdjacentMines.ToString();
                }
                return Cell.IsFlagged ? "F" : string.Empty;
            }
        }

        public string ImageSource
        {
            get
            {
                if (Cell.IsRevealed)
                {
                    return Cell.IsMine ? "Images/mine.png" : $"Images/{Cell.AdjacentMines}.png";
                }
                return Cell.IsFlagged ? "Images/flag.png" : "Images/none.png";
            }
        }
    }
}
