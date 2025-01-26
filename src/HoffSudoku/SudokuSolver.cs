using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace HoffSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        // Using int arrays for row, column, and box checks.
        // Each integer has up to 32 bits, so it's fine for 25x25.
        private int[] rowMask;
        private int[] colMask;
        private int[] boxMask;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;

            // Allocate masks
            rowMask = new int[n];
            colMask = new int[n];
            boxMask = new int[n];

            InitializeUsed();
        }

        private void InitializeUsed()
        {
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            // Go through each cell. If it has a single value,
            // set the correct bit in row/col/box masks.
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    HashSet<int> opts = board.GetOptions(r, c);
                    if (opts.Count == 1)
                    {
                        int val = opts.First();
                        int bitIndex = (val - minVal) / step;
                        SetBit(r, c, bitIndex);
                    }
                }
            }
        }

        // Helper to turn on the bit for row, col, and box.
        private void SetBit(int row, int col, int bitIndex)
        {
            rowMask[row] |= (1 << bitIndex);
            colMask[col] |= (1 << bitIndex);
            boxMask[GetBoxIndex(row, col)] |= (1 << bitIndex);
        }

        // Helper to turn off the bit for row, col, and box.
        private void ClearBit(int row, int col, int bitIndex)
        {
            rowMask[row] &= ~(1 << bitIndex);
            colMask[col] &= ~(1 << bitIndex);
            boxMask[GetBoxIndex(row, col)] &= ~(1 << bitIndex);
        }

        // Finds which box we're in based on row and col.
        private int GetBoxIndex(int r, int c)
        {
            return (r / SudokuConstants.SubgridRows) * SudokuConstants.SubgridCols
                   + (c / SudokuConstants.SubgridCols);
        }

        public bool Solve()
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool solved = Backtrack();
            sw.Stop();
            Console.WriteLine($"Time taken to solve: {sw.ElapsedMilliseconds} ms");
            return solved;
        }

        private bool Backtrack()
        {
            int n = SudokuConstants.BoardSize;

            // We'll choose the cell with the fewest valid options.
            int minOptionsCount = n + 1;
            int chosenRow = -1;
            int chosenCol = -1;

            // Find the best cell to branch on.
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    HashSet<int> opts = board.GetOptions(r, c);
                    if (opts.Count == 1) continue;

                    // Count how many are actually valid when checking row/col/box
                    int validCount = 0;
                    foreach (int val in opts)
                    {
                        int bitIndex = (val - SudokuConstants.MinValue) / SudokuConstants.Step;

                        // If the bit is already set in any mask, skip it
                        if (((rowMask[r] >> bitIndex) & 1) == 1) continue;
                        if (((colMask[c] >> bitIndex) & 1) == 1) continue;
                        if (((boxMask[GetBoxIndex(r, c)] >> bitIndex) & 1) == 1) continue;

                        validCount++;
                    }

                    // If no valid numbers, this path fails.
                    if (validCount == 0)
                    {
                        return false;
                    }

                    // Track cell with fewest possibilities.
                    if (validCount < minOptionsCount)
                    {
                        minOptionsCount = validCount;
                        chosenRow = r;
                        chosenCol = c;
                    }
                }
            }

            // If chosenRow is still -1, that means everything is single-valued.
            // We must be solved.
            if (chosenRow == -1 && chosenCol == -1)
            {
                return true;
            }

            // Try each valid possibility.
            HashSet<int> chosenCellOptions = board.GetOptions(chosenRow, chosenCol);
            foreach (int val in chosenCellOptions)
            {
                int bitIndex = (val - SudokuConstants.MinValue) / SudokuConstants.Step;

                // If any mask has this bit set, we can't use it.
                if (((rowMask[chosenRow] >> bitIndex) & 1) == 1) continue;
                if (((colMask[chosenCol] >> bitIndex) & 1) == 1) continue;
                if (((boxMask[GetBoxIndex(chosenRow, chosenCol)] >> bitIndex) & 1) == 1) continue;

                // Temporarily set the value in the board
                board.SetValue(chosenRow, chosenCol, val);
                SetBit(chosenRow, chosenCol, bitIndex);

                // Recurse
                if (Backtrack())
                {
                    return true;
                }

                // Undo changes if it didn't work
                board.SetValue(chosenRow, chosenCol, 0);
                ClearBit(chosenRow, chosenCol, bitIndex);
            }

            return false;
        }
    }
}
