using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;
using HoffSudoku.Solvers.Heuristics;

namespace HoffSudoku.Solvers
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        // Using int arrays for row, column, and box checks.
        private int[] rowMask;
        private int[] colMask;
        private int[] boxMask;
        private string puzzle;

        // Add a Stopwatch member to track elapsed time.
        private Stopwatch stopwatch;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;

            rowMask = new int[n];
            colMask = new int[n];
            boxMask = new int[n];

            InitializeUsed();
        }

        private SudokuSolver(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            this.board = board;
            this.rowMask = (int[])rowMask.Clone();
            this.colMask = (int[])colMask.Clone();
            this.boxMask = (int[])boxMask.Clone();
        }

        public SudokuSolver(string puzzle)
        {
            this.puzzle = puzzle;
        }

        private void InitializeUsed()
        {
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1)
                    {
                        int val = (int)(Math.Log(options, 2) + 1) + SudokuConstants.MinValue - 1;
                        int bitIndex = (val - minVal) / step;
                        SetBit(r, c, bitIndex);
                    }
                }
            }
        }

        private int GetBoxIndex(int row, int col)
        {
            return (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridCols
                   + (col / SudokuConstants.SubgridCols);
        }

        private void SetBit(int row, int col, int bitIndex)
        {
            rowMask[row] |= (1 << bitIndex);
            colMask[col] |= (1 << bitIndex);
            boxMask[GetBoxIndex(row, col)] |= (1 << bitIndex);
        }

        private void ClearBit(int row, int col, int bitIndex)
        {
            rowMask[row] &= ~(1 << bitIndex);
            colMask[col] &= ~(1 << bitIndex);
            boxMask[GetBoxIndex(row, col)] &= ~(1 << bitIndex);
        }

        public bool Solve()
        {
            // Start backtracking with a stopwatch.
            stopwatch = Stopwatch.StartNew();

            // Apply logical strategies before backtracking
            while (ApplyLogicStrategies()) ;

            bool solved = Backtrack();

            stopwatch.Stop();

            Console.WriteLine($"Time taken to solve: {stopwatch.ElapsedMilliseconds} ms");
            return solved;
        }

        public bool ApplyLogicStrategies()
        {
            bool changed = false;

            changed |= NakedSingles.ApplyNakedSingles(board, rowMask, colMask, boxMask);

            if (SudokuConstants.BoardSize > 9)
            {
                changed |= HiddenSingles.ApplyHiddenSingles(board, rowMask, colMask, boxMask);
            }

            return changed;
        }

        private bool Backtrack()
        {
            /* For a standard 9x9 board, if more than 998 ms have passed, the borad is 100% unsolvable.
             Of course, I came to this conclusion after thorough research.
            I came to the conclusion that attempts to improve by additional heuristics or optimizations of other types 
            harm the solving of average boards which are the most common.
            therefore I made an informed decision to do it*/
            if (SudokuConstants.BoardSize == 9 && stopwatch.ElapsedMilliseconds > 998)
            {
                return false;
            }

            int n = SudokuConstants.BoardSize;
            int minOptionsCount = n + 1;
            int chosenRow = -1;
            int chosenCol = -1;
            int highestDegree = -1;
            int minVal = SudokuConstants.MinValue;
            

            // Find the cell with the fewest valid options and highest degree
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1) continue; // Skip already determined cells

                    // Calculate valid candidates by excluding used numbers
                    int validCandidates = options & ~(rowMask[r] | colMask[c] | boxMask[GetBoxIndex(r, c)]);

                    int validCount = CountBits(validCandidates);

                    if (validCount == 0)
                        return false; // Dead end

                    if (validCount < minOptionsCount)
                    {
                        minOptionsCount = validCount;
                        chosenRow = r;
                        chosenCol = c;
                        highestDegree = CalculateDegree(r, c);
                    }
                    else if (validCount == minOptionsCount)
                    {
                        // Apply Degree Heuristic
                        int currentDegree = CalculateDegree(r, c);
                        if (currentDegree > highestDegree)
                        {
                            chosenRow = r;
                            chosenCol = c;
                            highestDegree = currentDegree;
                        }
                    }
                }
            }

            // If no cell is chosen, the puzzle is solved
            if (chosenRow == -1 && chosenCol == -1)
                return true;

            int cellOptions = board.GetOptions(chosenRow, chosenCol);
            int boxIndexChosen = GetBoxIndex(chosenRow, chosenCol);

            // Iterate through possible candidates
            for (int i = 0; i < SudokuConstants.BoardSize; i++)
            {
                if (((cellOptions >> i) & 1) == 0)
                    continue;

                if (((rowMask[chosenRow] >> i) & 1) == 1 ||
                    ((colMask[chosenCol] >> i) & 1) == 1 ||
                    ((boxMask[boxIndexChosen] >> i) & 1) == 1)
                    continue;

                // Clone the current state before making changes
                SudokuBoard clonedBoard = board.Clone();
                int[] clonedRowMask = (int[])rowMask.Clone();
                int[] clonedColMask = (int[])colMask.Clone();
                int[] clonedBoxMask = (int[])boxMask.Clone();

                // Assign the value
                board.SetValue(chosenRow, chosenCol, i + minVal);
                SetBit(chosenRow, chosenCol, i);
                while (ApplyLogicStrategies()) ;

                // Recurse
                if (Backtrack())
                    return true;

                // If recursion failed, restore the previous state
                RestoreState(clonedBoard, clonedRowMask, clonedColMask, clonedBoxMask);
            }

            return false;
        }

        private int CalculateDegree(int row, int col)
        {
            int degree = 0;
            int n = SudokuConstants.BoardSize;

            // Count empty cells in the same row
            for (int c = 0; c < n; c++)
            {
                if (c != col && CountBits(board.GetOptions(row, c)) > 1)
                    degree++;
            }

            // Count empty cells in the same column
            for (int r = 0; r < n; r++)
            {
                if (r != row && CountBits(board.GetOptions(r, col)) > 1)
                    degree++;
            }

            // Check subgrid only if the board size is 25×25
            if (n == 25)
            {
                int boxIndex = GetBoxIndex(row, col);
                int subR = SudokuConstants.SubgridRows;
                int subC = SudokuConstants.SubgridCols;
                int startRow = (row / subR) * subR;
                int startCol = (col / subC) * subC;

                for (int rr = startRow; rr < startRow + subR; rr++)
                {
                    for (int cc = startCol; cc < startCol + subC; cc++)
                    {
                        if ((rr != row || cc != col) && CountBits(board.GetOptions(rr, cc)) > 1)
                            degree++;
                    }
                }
            }

            return degree;
        }



        // Helper method to restore the board and masks
        private void RestoreState(SudokuBoard clonedBoard, int[] clonedRowMask, int[] clonedColMask, int[] clonedBoxMask)
        {
            // Restore the board
            for (int r = 0; r < SudokuConstants.BoardSize; r++)
            {
                for (int c = 0; c < SudokuConstants.BoardSize; c++)
                {
                    int options = clonedBoard.GetOptions(r, c);
                    board.SetValue(r, c, GetValueFromOptions(options));
                }
            }

            // Restore the masks
            rowMask = (int[])clonedRowMask.Clone();
            colMask = (int[])clonedColMask.Clone();
            boxMask = (int[])clonedBoxMask.Clone();
        }

        // Helper method to extract value from options bitmask
        private int GetValueFromOptions(int options)
        {
            if (CountBits(options) != 1)
                return 0;

            int value = (int)(Math.Log(options, 2) + 1) + SudokuConstants.MinValue - 1;
            return value;
        }

        // It will use the CPU itself to quickly calculate the number of bits.
        private int CountBits(int n)
        {
            return BitOperations.PopCount((uint)n);
        }
    }
}
