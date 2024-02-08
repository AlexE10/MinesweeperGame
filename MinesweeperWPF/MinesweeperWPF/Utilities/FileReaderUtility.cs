using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesweeperWPF.Models;

namespace MinesweeperWPF.Utilities
{
    public static class FileReaderUtility
    {
        public static IReadOnlyList<IReadOnlyList<Cell>> ReadMatrixFromFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var cellMatrix = lines.Select(line =>
                    line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => CreateCellFromInt(int.Parse(str)))
                        .ToList())
                    .ToList();

                // Convert to IReadOnlyList<IReadOnlyList<Cell>>
                return cellMatrix.Select(row => row.AsReadOnly()).ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error reading matrix from file.", ex);
            }
        }

        private static Cell CreateCellFromInt(int value)
        {
            bool isMine = value == -1;
            int adjacentMines = isMine ? 0 : value;
            return new Cell(isMine, false, false, adjacentMines);
        }
    }

}
