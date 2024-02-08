using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.Models
{
    public class Board
    {
        public IReadOnlyList<IReadOnlyList<Cell>> Cells { get; }

        public Board(IReadOnlyList<IReadOnlyList<Cell>> cells)
        {
            Cells = cells;
        }
    }

}
