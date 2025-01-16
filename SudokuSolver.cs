using System;
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
                    board.ResetOptions(row, col);
                }
            }

            // If no number fits, backtrack
            return false;
        }

        private bool IsValid(int row, int col, int num)
        {
            // Check if the number is already used in the row, column, or box
            return !board.GetUsedInRow(row).Contains(num) &&
                   !board.GetUsedInCol(col).Contains(num) &&
                   !board.GetUsedInBox(row, col).Contains(num);
        }
    }
}
