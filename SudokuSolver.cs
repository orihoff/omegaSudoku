using System;
using System.Collections.Generic;

namespace omegaSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
        }

        public bool Solve()
        {
            return Backtrack(0, 0);
        }

        private bool Backtrack(int row, int col)
        {
            //Sudoku is solved
            if (row == SudokuConstants.BoardSize)
                return true;

            // Move to the next cell
            int nextRow = col == SudokuConstants.BoardSize - 1 ? row + 1 : row;
            int nextCol = (col + 1) % SudokuConstants.BoardSize;

            // If the cell already has a value, skip to next cell
            if (board.GetOptions(row, col).Count == 1)
                return Backtrack(nextRow, nextCol);

            // Try every possible number for this cell
            for (int num = SudokuConstants.MinValue; num <= SudokuConstants.MaxValue; num++)
            {
                if (IsValid(row, col, num))
                {
                    // Set the current number in the cell
                    board.GetOptions(row, col).Clear();
                    board.GetOptions(row, col).Add(num);

                    // Recursive call to solve the rest of the board
                    if (Backtrack(nextRow, nextCol))
                        return true;

                    // If it didn't work, reset the cell (backtracking)
                    board.GetOptions(row, col).Clear();
                }
            }

            return false; // If no solution was found, backtrack
        }

        private bool IsValid(int row, int col, int num)
        {
            // Check if the number is valid in the row column and 3x3 box
            return !GetUsedInRow(row).Contains(num) &&
                   !GetUsedInCol(col).Contains(num) &&
                   !GetUsedInBox(row, col).Contains(num);
        }

        private HashSet<int> GetUsedInRow(int row)
        {
            var used = new HashSet<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]); // Add value if the cell is already filled
            }
            return used;
        }

        private HashSet<int> GetUsedInCol(int col)
        {
            var used = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]); // Add  value if the cell is already filled
            }
            return used;
        }

        private HashSet<int> GetUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRowStart = (row / 3) * 3; // Find the starting row of the 3x3 box
            int boxColStart = (col / 3) * 3; // Find the starting column ...

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var options = board.GetOptions(boxRowStart + r, boxColStart + c);
                    if (options.Count == 1)
                        used.Add(options[0]); // Add the value if the cell is already filled
                }
            }
            return used;
        }
    }
}
