using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace omegaSudoku
{
    public class SudokuSolver
    {
        private SudokuBoard board;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
        }

        public bool Solve()
        {
            var stopwatch = Stopwatch.StartNew(); // Start timing

            bool solved = Backtrack(0, 0);

            stopwatch.Stop(); // Stop timing
            Console.WriteLine($"Time taken to solve: {stopwatch.ElapsedMilliseconds} ms");

            return solved;
        }

        private bool Backtrack(int row, int col)
        {
            // If we've reached the end of the board, we're done
            if (row == SudokuConstants.BoardSize)
                return true;

            // Determine the next cell
            int nextRow = col == SudokuConstants.BoardSize - 1 ? row + 1 : row;
            int nextCol = (col + 1) % SudokuConstants.BoardSize;

            // If the cell is already filled, move to the next one
            if (board.GetOptions(row, col).Count == 1)
                return Backtrack(nextRow, nextCol);

            // Try every possible value for the current cell
            for (int num = SudokuConstants.MinValue;
                 num < SudokuConstants.MinValue + SudokuConstants.BoardSize * SudokuConstants.Step;
                 num += SudokuConstants.Step)
            {
                if (IsValid(row, col, num))
                {
                    // Update the cell with the chosen value
                    board.GetOptions(row, col).Clear();
                    board.GetOptions(row, col).Add(num);

                    // Recursively solve the rest of the board
                    if (Backtrack(nextRow, nextCol))
                        return true;

                    // If it fails, reset the cell
                    board.GetOptions(row, col).Clear();
                    ResetOptions(row, col);
                }
            }

            // If no number fits, backtrack
            return false;
        }

        private void ResetOptions(int row, int col)
        {
            for (int i = SudokuConstants.MinValue;
                 i < SudokuConstants.MinValue + SudokuConstants.BoardSize * SudokuConstants.Step;
                 i += SudokuConstants.Step)
            {
                board.GetOptions(row, col).Add(i);
            }
        }

        private bool IsValid(int row, int col, int num)
        {
            // Check if the number is already used in the row, column, or box
            return !getUsedInRow(row).Contains(num) &&
                   !getUsedInCol(col).Contains(num) &&
                   !getUsedInBox(row, col).Contains(num);
        }

        private IEnumerable<int> getUsedInRow(int row)
        {
            var used = new HashSet<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        private IEnumerable<int> getUsedInCol(int col)
        {
            var used = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        private IEnumerable<int> getUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRow = (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridRows;
            int boxCol = (col / SudokuConstants.SubgridCols) * SudokuConstants.SubgridCols;

            for (int r = 0; r < SudokuConstants.SubgridRows; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                {
                    var options = board.GetOptions(boxRow + r, boxCol + c);
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
            }
            return used;
        }
    }
}
