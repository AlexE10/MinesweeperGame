using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MinesweeperWPF.Utilities;
using MinesweeperWPF.ViewModels;

namespace MinesweeperWPF.Models
{
    public static class GameLogic
    {
        public static List<TOutput> Map<TInput, TOutput>(List<TInput> inputList, Func<TInput, TOutput> transformation)
        {
            var outputList = new List<TOutput>();
            foreach (var item in inputList)
            {
                outputList.Add(transformation(item));
            }
            return outputList;
        }
        public static GameState RevealCell(GameState currentState, int x, int y)
        {
            var newState = CloneGameState(currentState);
            int availableFlags = newState.AvailableFlags;

            if (!IsValidCell(newState.Board.Cells, x, y) || newState.Board.Cells[x][y].IsRevealed || newState.GameOver || newState.GameWon)
            {
                return newState; 
            }

            var (updatedBoard, flagChange) = UpdateBoard(newState.Board, x, y);
            availableFlags += flagChange;

            bool gameOver = updatedBoard.Cells[x][y].IsMine;

            return new GameState(
                updatedBoard,
                availableFlags,
                gameOver,
                gameOver ? false : CheckForWin(updatedBoard));
        }

        public static GameState CloneGameState(GameState originalState)
        {
            var clonedBoard = CloneBoard(originalState.Board);

            var clonedState = new GameState(
                clonedBoard,
                originalState.AvailableFlags,
                originalState.GameOver,
                originalState.GameWon);

            return clonedState;
        }

        private static Board CloneBoard(Board originalBoard)
        {
            var clonedCells = originalBoard.Cells
                .Select(row => row.Select(cell => CloneCell(cell)).ToList())
                .ToList();

            return new Board(clonedCells);
        }

        private static Cell CloneCell(Cell originalCell)
        {
            return new Cell(
                originalCell.IsMine,
                originalCell.IsRevealed,
                originalCell.IsFlagged,
                originalCell.AdjacentMines);
        }

        public static (Board, int) UpdateBoard(Board currentBoard, int x, int y)
        {
            int flagChange = 0;
            //var newCells = currentBoard.Cells
            //    .Select(row => row.Select(cell => CloneCell(cell)).ToList())
            //    .ToList();
            var newCells = currentBoard.Cells.Select(row => Map<Cell, Cell>(row.ToList(), cell => CloneCell(cell))).ToList();

            var (revealedCell, cellFlagChange) = RevealCell(newCells[x][y]);
            newCells[x][y] = revealedCell;
            flagChange += cellFlagChange;

            if (newCells[x][y].AdjacentMines == 0)
            {
                var (updatedCells, neighborsFlagChange) = RevealNeighbors(newCells, x, y);
                newCells = updatedCells;
                flagChange += neighborsFlagChange;
            }

            return (new Board(newCells), flagChange);
        }

        private static (Cell, int) RevealCell(Cell cell)
        {
            int flagChange = cell.IsFlagged && !cell.IsRevealed ? 1 : 0;
            var newCell = new Cell(cell.IsMine, true, false, cell.AdjacentMines);
            return (newCell, flagChange);
        }


        private static (List<List<Cell>>, int) RevealNeighbors(List<List<Cell>> cells, int x, int y)
        {
            int flagChange = 0;
            int neighborsFlagChange = 0;
            var neighborPositions = new List<(int, int)>
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1),            (0, 1),
        (1, -1), (1, 0), (1, 1)
    };

            foreach (var (dx, dy) in neighborPositions)
            {
                int newX = x + dx, newY = y + dy;
                if (IsValidCell(cells, newX, newY) && !cells[newX][newY].IsRevealed)
                {
                    var (revealedCell, cellFlagChange) = RevealCell(cells[newX][newY]);
                    cells[newX][newY] = revealedCell;
                    flagChange += cellFlagChange;

                    if (cells[newX][newY].AdjacentMines == 0)
                    {
                        (cells, var neighborFlags) = RevealNeighbors(cells, newX, newY);
                        neighborsFlagChange += neighborFlags;
                    }
                }
            }
            flagChange += neighborsFlagChange;
            return (cells, flagChange);
        }


        private static bool IsValidCell(IReadOnlyList<IReadOnlyList<Cell>> cells, int x, int y)
        {
            return x >= 0 && y >= 0 && x < cells.Count && y < cells[x].Count;
        }


        public static bool CheckForWin(Board board)
        {
            return board.Cells.All(row => row.All(cell =>
                cell.IsMine || cell.IsRevealed));
        }

        public static GameState CreateInitialState(int difficulty)
        {
            string filePath = "../../../PrologMap/matrix_output.txt";
            int availableFlags = PrologToTxtFile.CreateMatrixInfile(difficulty);
            var cellMatrix = FileReaderUtility.ReadMatrixFromFile(filePath);

            var board = new Board(cellMatrix);

            return new GameState(board, availableFlags, gameOver: false, gameWon: false);
        }
        //public static CellPosition GetSafeCellHint(GameState currentState)
        //{
        //    // Iterate through all cells to find a safe cell
        //    for (int x = 0; x < currentState.Board.Cells.Count; x++)
        //    {
        //        for (int y = 0; y < currentState.Board.Cells[x].Count; y++)
        //        {
        //            var cell = currentState.Board.Cells[x][y];

        //            // Check if the cell is not revealed and not flagged, and is not a mine
        //            if (!cell.IsRevealed && !cell.IsFlagged && !cell.IsMine)
        //            {
        //                // This cell is safe to click
        //                return new CellPosition(x, y);
        //            }
        //        }
        //    }

        //    // No safe cell found, return null or an indication that no hint is available
        //    return null;
        //}

        public static GameState ToggleFlagOnCell(GameState currentState, int x, int y)
        {
            if (!IsValidCell(currentState.Board.Cells, x, y) || currentState.Board.Cells[x][y].IsRevealed)
            {
                return currentState; 
            }

            var (newCells, flagsChange) = CloneAndToggleFlag(currentState.Board.Cells, x, y);

            var newBoard = new Board(newCells);

            return new GameState(newBoard, currentState.AvailableFlags + flagsChange, currentState.GameOver, currentState.GameWon);
        }

        private static (IReadOnlyList<IReadOnlyList<Cell>>, int) CloneAndToggleFlag(
            IReadOnlyList<IReadOnlyList<Cell>> cells, int x, int y)
        {
            var newCells = cells.Select((row, rowIndex) =>
                rowIndex == x
                    ? row.Select((cell, colIndex) => colIndex == y
                        ? new Cell(cell.IsMine, cell.IsRevealed, !cell.IsFlagged, cell.AdjacentMines)
                        : cell).ToList()
                    : row.ToList()).ToList();

            return (newCells.Select(row => row.AsReadOnly()).ToList().AsReadOnly(), cells[x][y].IsFlagged ? 1 : -1);
        }

        public static GameState RevealAllBombs(GameState currentState)
        {
            if (!currentState.GameOver && !currentState.GameWon)
            {
                return currentState;
            }

            var newCells = currentState.Board.Cells
                .Select(row => row.Select(cell => cell.IsMine
                    ? new Cell(cell.IsMine, true, cell.IsFlagged, cell.AdjacentMines)
                    : cell).ToList())
                .ToList();

            var newBoard = new Board(newCells);

            return new GameState(newBoard, 0, currentState.GameOver, currentState.GameWon);
        }
    }
}
