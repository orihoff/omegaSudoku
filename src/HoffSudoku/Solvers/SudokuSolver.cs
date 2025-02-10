using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;
using HoffSudoku.Solvers.Heuristics;
using HoffSudoku.Helpers; 

namespace HoffSudoku.Solvers
{
    /// <summary>
    /// A Sudoku solver that uses heuristics and backtracking.
    /// </summary>
    public class SudokuSolver
    {
        private readonly SudokuBoard board;
        private int[] rowMask;
        private int[] colMask;
        private int[] boxMask;
        private string puzzle;
        private Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the SudokuSolver class with a given board.
        /// </summary>
        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;

            // Initialize masks for rows, columns, and boxes.
            rowMask = new int[n];
            colMask = new int[n];
            boxMask = new int[n];

            InitializeUsed(); // Set up initial mask state based on given board.
        }

        /// <summary>
        /// Initializes a new instance of the SudokuSolver class with a board and precomputed masks.
        /// </summary>
        private SudokuSolver(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            this.board = board;
            // Clone the mask arrays to preserve state.
            this.rowMask = (int[])rowMask.Clone();
            this.colMask = (int[])colMask.Clone();
            this.boxMask = (int[])boxMask.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the SudokuSolver class using a puzzle string.
        /// </summary>
        public SudokuSolver(string puzzle)
        {
            this.puzzle = puzzle;
        }

        /// <summary>
        /// Initializes the board's used values and updates the masks.
        /// </summary>
        private void InitializeUsed()
        {
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            // Loop through each cell on the board.
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    // Check if the cell has a determined value.
                    if (SudokuHelper.CountBits(options) == 1)
                    {
                        // Calculate the value's bit index.
                        int val = (int)(Math.Log(options, 2) + 1) + minVal - 1;
                        int bitIndex = (val - minVal) / step;
                        SudokuHelper.SetBit(r, c, bitIndex, rowMask, colMask, boxMask, SudokuHelper.GetBoxIndex(r, c));
                    }
                }
            }
        }

        /// <summary>
        /// Sets a bit in the corresponding row, column, and box masks.
        /// </summary>
        private void SetBit(int row, int col, int bitIndex)
        {
            SudokuHelper.SetBit(row, col, bitIndex, rowMask, colMask, boxMask, SudokuHelper.GetBoxIndex(row, col));
        }

        /// <summary>
        /// Clears a bit in the corresponding row, column, and box masks.
        /// </summary>
        private void ClearBit(int row, int col, int bitIndex)
        {
            // Since the helper does not provide a ClearBit, נממש כאן את הפעולה.
            rowMask[row] &= ~(1 << bitIndex);
            colMask[col] &= ~(1 << bitIndex);
            boxMask[SudokuHelper.GetBoxIndex(row, col)] &= ~(1 << bitIndex);
        }

        /// <summary>
        /// Attempts to solve the Sudoku puzzle using logical strategies and backtracking.
        /// </summary>
        public bool Solve()
        {
            stopwatch = Stopwatch.StartNew(); // Start timing.

            while (ApplyLogicStrategies()) ; // Apply logic repeatedly until no changes.

            bool solved = Backtrack(); // Begin backtracking.

            stopwatch.Stop();
            Console.WriteLine($"Time taken to solve: {stopwatch.ElapsedMilliseconds} ms");
            return solved;
        }

        /// <summary>
        /// Applies logical solving strategies NakedSingles and HiddenSingles.
        /// </summary>
        public bool ApplyLogicStrategies()
        {
            bool changed = false;

            // Apply Naked Singles heuristic.
            changed |= NakedSingles.ApplyNakedSingles(board, rowMask, colMask, boxMask);

            // For larger boards, try Hidden Singles.
            if (SudokuConstants.BoardSize > 9)
            {
                changed |= HiddenSingles.ApplyHiddenSingles(board, rowMask, colMask, boxMask);
            }

            return changed;
        }

        /// <summary>
        /// Uses backtracking to search for a solution to the Sudoku puzzle.
        /// </summary>
        private bool Backtrack()
        {
            // Check Hall condition before proceeding.
            if (!HallConditionValidator.ValidateHallCondition(board, rowMask, colMask, boxMask))
                return false;

            int n = SudokuConstants.BoardSize;
            int minOptionsCount = n + 1;
            int chosenRow = -1;
            int chosenCol = -1;
            int highestDegree = -1;
            int minVal = SudokuConstants.MinValue;

            // Find the cell with the fewest valid candidates.
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1)
                        continue; // Skip solved cells.

                    int validCandidates = options & ~(rowMask[r] | colMask[c] | boxMask[SudokuHelper.GetBoxIndex(r, c)]);
                    int validCount = SudokuHelper.CountBits(validCandidates);

                    if (validCount == 0)
                        return false; // Dead end: no candidates.

                    // Choose the cell with minimal candidates and highest degree.
                    if (validCount < minOptionsCount)
                    {
                        minOptionsCount = validCount;
                        chosenRow = r;
                        chosenCol = c;
                        highestDegree = CalculateDegree(r, c);
                    }
                    else if (validCount == minOptionsCount)
                    {
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

            // If no cell is selected, the puzzle is solved.
            if (chosenRow == -1 && chosenCol == -1)
                return true;

            int cellOptions = board.GetOptions(chosenRow, chosenCol);
            int boxIndexChosen = SudokuHelper.GetBoxIndex(chosenRow, chosenCol);

            // Try each candidate for the chosen cell.
            for (int i = 0; i < SudokuConstants.BoardSize; i++)
            {
                if (((cellOptions >> i) & 1) == 0)
                    continue; // Candidate not available.

                // Skip candidate if already used in row, column, or box.
                if (((rowMask[chosenRow] >> i) & 1) == 1 ||
                    ((colMask[chosenCol] >> i) & 1) == 1 ||
                    ((boxMask[boxIndexChosen] >> i) & 1) == 1)
                    continue;

                // Clone the current state for backtracking.
                SudokuBoard clonedBoard = board.Clone();
                int[] clonedRowMask = (int[])rowMask.Clone();
                int[] clonedColMask = (int[])colMask.Clone();
                int[] clonedBoxMask = (int[])boxMask.Clone();

                board.SetValue(chosenRow, chosenCol, i + minVal); // Place candidate.
                SetBit(chosenRow, chosenCol, i); // Update masks.
                while (ApplyLogicStrategies()) ; // Apply logic after assignment.

                if (Backtrack())
                    return true; // Solution found.

                // Restore previous state if candidate fails.
                RestoreState(clonedBoard, clonedRowMask, clonedColMask, clonedBoxMask);
            }

            return false; // No candidate led to a solution.
        }

        /// <summary>
        /// Calculates the degree of a cell based on the number of unsolved neighboring cells.
        /// </summary>
        private int CalculateDegree(int row, int col)
        {
            int degree = 0;
            int n = SudokuConstants.BoardSize;

            // Count unsolved cells in the same row.
            for (int c = 0; c < n; c++)
            {
                if (c != col && SudokuHelper.CountBits(board.GetOptions(row, c)) > 1)
                    degree++;
            }

            // Count unsolved cells in the same column.
            for (int r = 0; r < n; r++)
            {
                if (r != row && SudokuHelper.CountBits(board.GetOptions(r, col)) > 1)
                    degree++;
            }

            // For 25x25 boards, also check the subgrid.
            if (n == 25)
            {
                int subR = SudokuConstants.SubgridRows;
                int subC = SudokuConstants.SubgridCols;
                int startRow = (row / subR) * subR;
                int startCol = (col / subC) * subC;

                for (int rr = startRow; rr < startRow + subR; rr++)
                {
                    for (int cc = startCol; cc < startCol + subC; cc++)
                    {
                        if ((rr != row || cc != col) && SudokuHelper.CountBits(board.GetOptions(rr, cc)) > 1)
                            degree++;
                    }
                }
            }

            return degree;
        }

        /// <summary>
        /// Restores the board and masks from previously cloned states.
        /// </summary>
        private void RestoreState(SudokuBoard clonedBoard, int[] clonedRowMask, int[] clonedColMask, int[] clonedBoxMask)
        {
            // Restore board values from the clone.
            for (int r = 0; r < SudokuConstants.BoardSize; r++)
            {
                for (int c = 0; c < SudokuConstants.BoardSize; c++)
                {
                    int options = clonedBoard.GetOptions(r, c);
                    board.SetValue(r, c, GetValueFromOptions(options));
                }
            }

            // Restore masks.
            rowMask = (int[])clonedRowMask.Clone();
            colMask = (int[])clonedColMask.Clone();
            boxMask = (int[])clonedBoxMask.Clone();
        }

        /// <summary>
        /// Extracts a determined value from the options bitmask.
        /// </summary>
        private int GetValueFromOptions(int options)
        {
            // Return value only if cell is solved.
            if (SudokuHelper.CountBits(options) != 1)
                return 0;

            int value = (int)(Math.Log(options, 2) + 1) + SudokuConstants.MinValue - 1;
            return value;
        }
    }
}
