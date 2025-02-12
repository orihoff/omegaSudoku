using System;
using System.Diagnostics;
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

        // Enum to represent the result of selecting the next unsolved cell.
        // This enum is used internally by SudokuSolver only, so I decided to not extracted it into a separate file.

        private enum CellSelectionResult
        {
            Solved,   // No unsolved cells – puzzle solved.
            Found,    // Found an unsolved cell to try.
            DeadEnd   // Found a cell with no valid candidates.
        }

        /// <summary>
        /// Initializes a new instance of the SudokuSolver class.
        /// </summary>
        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;
            rowMask = new int[n];
            colMask = new int[n];
            boxMask = new int[n];
            InitializeUsed();
        }

        /// <summary>
        /// Initializes a new instance with precomputed masks.
        /// </summary>
        private SudokuSolver(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            this.board = board;
            // Clone the masks to preserve state
            this.rowMask = (int[])rowMask.Clone();
            this.colMask = (int[])colMask.Clone();
            this.boxMask = (int[])boxMask.Clone();
        }

        /// <summary>
        /// Initializes a new instance using a puzzle string.
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

            // Loop over every cell to set initial state
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1)
                    {
                        // Compute the bit index for the determined value
                        int val = (int)(Math.Log(options, 2) + 1) + minVal - 1;
                        int bitIndex = (val - minVal) / step;
                        // Update the masks with the found value
                        SudokuHelper.SetBit(r, c, bitIndex, rowMask, colMask, boxMask, SudokuHelper.GetBoxIndex(r, c));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the specified bit in the corresponding masks.
        /// </summary>
        private void SetBit(int row, int col, int bitIndex)
        {
            // Mark the candidate as used in the row, column, and box masks
            SudokuHelper.SetBit(row, col, bitIndex, rowMask, colMask, boxMask, SudokuHelper.GetBoxIndex(row, col));
        }

        /// <summary>
        /// Clears the specified bit in the corresponding masks.
        /// </summary>
        private void ClearBit(int row, int col, int bitIndex)
        {
            // Remove the candidate from the masks
            rowMask[row] &= ~(1 << bitIndex);
            colMask[col] &= ~(1 << bitIndex);
            boxMask[SudokuHelper.GetBoxIndex(row, col)] &= ~(1 << bitIndex);
        }

        /// <summary>
        /// Attempts to solve the Sudoku puzzle.
        /// </summary>
        public bool Solve()
        {
            stopwatch = Stopwatch.StartNew();

            // Continuously apply logic strategies until no further progress
            while (ApplyLogicStrategies()) ;

            // Start backtracking search
            bool solved = Backtrack();

            stopwatch.Stop();
            Console.WriteLine($"Time taken to solve: {stopwatch.ElapsedMilliseconds} ms");
            return solved;
        }

        /// <summary>
        /// Applies logical strategies to the board.
        /// </summary>
        public bool ApplyLogicStrategies()
        {
            bool changed = false;
            // Apply the NakedSingles heuristic
            changed |= NakedSingles.ApplyNakedSingles(board, rowMask, colMask, boxMask);
            // For larger puzzles, apply HiddenSingles heuristic as well
            if (SudokuConstants.BoardSize > 9)
            {
                changed |= HiddenSingles.ApplyHiddenSingles(board, rowMask, colMask, boxMask);
            }
            return changed;
        }

        /// <summary>
        /// Recursively searches for a valid solution using backtracking.
        /// </summary>
        private bool Backtrack()
        {
            // Check necessary conditions before proceeding
            if (!HallConditionValidator.ValidateHallCondition(board, rowMask, colMask, boxMask))
                return false;

            int chosenRow, chosenCol, cellOptions;
            // Select the next cell that needs to be solved
            CellSelectionResult selectionResult = SelectUnsolvedCell(out chosenRow, out chosenCol, out cellOptions);

            // If no valid candidate exists, return false
            if (selectionResult == CellSelectionResult.DeadEnd)
                return false;

            // If all cells are solved, return true
            if (selectionResult == CellSelectionResult.Solved)
                return true;

            // Try each candidate for the selected cell
            return TryCandidates(chosenRow, chosenCol, cellOptions);
        }

        /// <summary>
        /// Selects the next unsolved cell.
        /// </summary>
        private CellSelectionResult SelectUnsolvedCell(out int chosenRow, out int chosenCol, out int cellOptions)
        {
            int n = SudokuConstants.BoardSize;
            int minOptionsCount = n + 1;
            chosenRow = -1;
            chosenCol = -1;
            cellOptions = 0;
            int highestDegree = -1;
            int minVal = SudokuConstants.MinValue;

            // Iterate over all cells to find the one with the fewest candidates
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1)
                        continue; // Skip already solved cells

                    // Filter out candidates already used in related masks
                    int validCandidates = options & ~(rowMask[r] | colMask[c] | boxMask[SudokuHelper.GetBoxIndex(r, c)]);
                    int validCount = SudokuHelper.CountBits(validCandidates);

                    // If no candidate is valid, it's a dead end
                    if (validCount == 0)
                        return CellSelectionResult.DeadEnd;

                    // Choose the cell with the minimum number of candidates
                    if (validCount < minOptionsCount)
                    {
                        minOptionsCount = validCount;
                        chosenRow = r;
                        chosenCol = c;
                        highestDegree = CalculateDegree(r, c);
                        cellOptions = options;
                    }
                    // If tied, choose the one with the highest degree (more constraints)
                    else if (validCount == minOptionsCount)
                    {
                        int currentDegree = CalculateDegree(r, c);
                        if (currentDegree > highestDegree)
                        {
                            chosenRow = r;
                            chosenCol = c;
                            highestDegree = currentDegree;
                            cellOptions = options;
                        }
                    }
                }
            }

            // If no cell was chosen, then all cells are solved
            if (chosenRow == -1)
                return CellSelectionResult.Solved;
            return CellSelectionResult.Found;
        }

        /// <summary>
        /// Tries each candidate for the selected cell.
        /// </summary>
        private bool TryCandidates(int chosenRow, int chosenCol, int cellOptions)
        {
            int boxIndexChosen = SudokuHelper.GetBoxIndex(chosenRow, chosenCol);
            int minVal = SudokuConstants.MinValue;

            // Iterate through each potential candidate
            for (int i = 0; i < SudokuConstants.BoardSize; i++)
            {
                // Skip if candidate is not available in the bitmask
                if (((cellOptions >> i) & 1) == 0)
                    continue;
                // Check if candidate is already used in row, column, or box
                if (((rowMask[chosenRow] >> i) & 1) == 1 ||
                    ((colMask[chosenCol] >> i) & 1) == 1 ||
                    ((boxMask[boxIndexChosen] >> i) & 1) == 1)
                    continue;

                // Clone current state for backtracking
                SudokuBoard clonedBoard = board.Clone();
                int[] clonedRowMask = (int[])rowMask.Clone();
                int[] clonedColMask = (int[])colMask.Clone();
                int[] clonedBoxMask = (int[])boxMask.Clone();

                // Set the candidate value in the board and update masks
                board.SetValue(chosenRow, chosenCol, i + minVal);
                SetBit(chosenRow, chosenCol, i);

                // Apply logic strategies after assignment
                while (ApplyLogicStrategies()) ;

                // Recurse; if solution found, return true
                if (Backtrack())
                    return true;

                // If candidate fails, restore previous state
                RestoreState(clonedBoard, clonedRowMask, clonedColMask, clonedBoxMask);
            }
            return false;
        }

        /// <summary>
        /// Calculates the degree of a cell.
        /// </summary>
        private int CalculateDegree(int row, int col)
        {
            int degree = 0;
            int n = SudokuConstants.BoardSize;

            // Count unsolved cells in the same row
            for (int c = 0; c < n; c++)
            {
                if (c != col && SudokuHelper.CountBits(board.GetOptions(row, c)) > 1)
                    degree++;
            }
            // Count unsolved cells in the same column
            for (int r = 0; r < n; r++)
            {
                if (r != row && SudokuHelper.CountBits(board.GetOptions(r, col)) > 1)
                    degree++;
            }
            // For 25x25 boards, include unsolved cells in the same subgrid
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
        /// Restores the board and masks from a cloned state.
        /// </summary>
        private void RestoreState(SudokuBoard clonedBoard, int[] clonedRowMask, int[] clonedColMask, int[] clonedBoxMask)
        {
            // Restore each cell from the cloned board
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

        /// <summary>
        /// Extracts the determined value from the options bitmask.
        /// </summary>
        private int GetValueFromOptions(int options)
        {
            if (SudokuHelper.CountBits(options) != 1)
                return 0;
            int value = (int)(Math.Log(options, 2) + 1) + SudokuConstants.MinValue - 1;
            return value;
        }
    }
}
