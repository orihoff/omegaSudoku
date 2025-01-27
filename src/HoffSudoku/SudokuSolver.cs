using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace HoffSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        // Using int arrays for row, column, and box checks.
        private int[] rowMask;
        private int[] colMask;
        private int[] boxMask;
        private string puzzle;

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
            // Apply logical strategies before backtracking
            while(ApplyLogicStrategies());

            // Start backtracking
            Stopwatch sw = Stopwatch.StartNew();
            bool solved = Backtrack();
            sw.Stop();

            Console.WriteLine($"Time taken to solve: {sw.ElapsedMilliseconds} ms");
            return solved;
        }

        public bool ApplyLogicStrategies()
        {
            bool changed = false;

            changed |= ApplyNakedSingles();

            if (SudokuConstants.BoardSize > 9)
            {
                changed |= ApplyHiddenSingles();
            }

            return changed;
        }
        private bool ApplyNakedSingles()
        {
            bool changed = false;
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1) continue; // Already determined

                    // Calculate box index once per cell
                    int boxIndex = GetBoxIndex(r, c);

                    // Efficiently identify single candidates using bitwise operations
                    int possibleValues = options & ~(rowMask[r] | colMask[c] | boxMask[boxIndex]);

                    if (CountBits(possibleValues) == 1)
                    {
                        // Extract the single valid value
                        int singleBit = possibleValues & -possibleValues; // Isolate the lowest set bit
                        int valIndex = BitOperations.Log2((uint)singleBit);
                        int val = valIndex + minVal;

                        // Set the value and update masks
                        board.SetValue(r, c, val);
                        SetBit(r, c, valIndex);
                        changed = true;
                    }
                }
            }

            return changed;
        }

        private bool ApplyHiddenSingles()
        {
            bool changed = false;
            changed |= ApplyHiddenSinglesForRows();
            changed |= ApplyHiddenSinglesForCols();
            changed |= ApplyHiddenSinglesForBoxes();
            return changed;
        }

        private bool ApplyHiddenSinglesForRows()
        {
            bool changed = false;
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int r = 0; r < n; r++)
            {
                Dictionary<int, int> valueCount = new Dictionary<int, int>();

                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1) continue;

                    for (int i = 0; i < n; i++)
                    {
                        if (((options >> i) & 1) == 1 &&
                            ((rowMask[r] >> i) & 1) == 0 &&
                            ((colMask[c] >> i) & 1) == 0 &&
                            ((boxMask[GetBoxIndex(r, c)] >> i) & 1) == 0)
                        {
                            if (valueCount.ContainsKey(i))
                                valueCount[i]++;
                            else
                                valueCount[i] = 1;
                        }
                    }
                }

                foreach (var kvp in valueCount)
                {
                    if (kvp.Value == 1)
                    {
                        int val = kvp.Key + minVal;
                        // Find the cell that can take this value
                        for (int c = 0; c < n; c++)
                        {
                            int options = board.GetOptions(r, c);
                            if (CountBits(options) == 1) continue;

                            if (((options >> kvp.Key) & 1) == 1 &&
                                ((rowMask[r] >> kvp.Key) & 1) == 0 &&
                                ((colMask[c] >> kvp.Key) & 1) == 0 &&
                                ((boxMask[GetBoxIndex(r, c)] >> kvp.Key) & 1) == 0)
                            {
                                board.SetValue(r, c, val);
                                SetBit(r, c, kvp.Key);
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        private bool ApplyHiddenSinglesForCols()
        {
            bool changed = false;
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int c = 0; c < n; c++)
            {
                Dictionary<int, int> valueCount = new Dictionary<int, int>();

                for (int r = 0; r < n; r++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1) continue;

                    for (int i = 0; i < n; i++)
                    {
                        if (((options >> i) & 1) == 1 &&
                            ((rowMask[r] >> i) & 1) == 0 &&
                            ((colMask[c] >> i) & 1) == 0 &&
                            ((boxMask[GetBoxIndex(r, c)] >> i) & 1) == 0)
                        {
                            if (valueCount.ContainsKey(i))
                                valueCount[i]++;
                            else
                                valueCount[i] = 1;
                        }
                    }
                }

                foreach (var kvp in valueCount)
                {
                    if (kvp.Value == 1)
                    {
                        int val = kvp.Key + minVal;
                        // Find the cell that can take this value
                        for (int r = 0; r < n; r++)
                        {
                            int options = board.GetOptions(r, c);
                            if (CountBits(options) == 1) continue;

                            if (((options >> kvp.Key) & 1) == 1 &&
                                ((rowMask[r] >> kvp.Key) & 1) == 0 &&
                                ((colMask[c] >> kvp.Key) & 1) == 0 &&
                                ((boxMask[GetBoxIndex(r, c)] >> kvp.Key) & 1) == 0)
                            {
                                board.SetValue(r, c, val);
                                SetBit(r, c, kvp.Key);
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        private bool ApplyHiddenSinglesForBoxes()
        {
            bool changed = false;
            int n = SudokuConstants.BoardSize;
            int subR = SudokuConstants.SubgridRows;
            int subC = SudokuConstants.SubgridCols;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int br = 0; br < n; br += subR)
            {
                for (int bc = 0; bc < n; bc += subC)
                {
                    Dictionary<int, int> valueCount = new Dictionary<int, int>();

                    for (int r = 0; r < subR; r++)
                    {
                        for (int c = 0; c < subC; c++)
                        {
                            int rr = br + r;
                            int cc = bc + c;
                            int options = board.GetOptions(rr, cc);
                            if (CountBits(options) == 1) continue;

                            for (int i = 0; i < n; i++)
                            {
                                if (((options >> i) & 1) == 1 &&
                                    ((rowMask[rr] >> i) & 1) == 0 &&
                                    ((colMask[cc] >> i) & 1) == 0 &&
                                    ((boxMask[GetBoxIndex(rr, cc)] >> i) & 1) == 0)
                                {
                                    if (valueCount.ContainsKey(i))
                                        valueCount[i]++;
                                    else
                                        valueCount[i] = 1;
                                }
                            }
                        }
                    }

                    foreach (var kvp in valueCount)
                    {
                        if (kvp.Value == 1)
                        {
                            int val = kvp.Key + minVal;
                            // Find the cell that can take this value
                            for (int r = 0; r < subR; r++)
                            {
                                for (int c = 0; c < subC; c++)
                                {
                                    int rr = br + r;
                                    int cc = bc + c;
                                    int options = board.GetOptions(rr, cc);
                                    if (CountBits(options) == 1) continue;

                                    if (((options >> kvp.Key) & 1) == 1 &&
                                        ((rowMask[rr] >> kvp.Key) & 1) == 0 &&
                                        ((colMask[cc] >> kvp.Key) & 1) == 0 &&
                                        ((boxMask[GetBoxIndex(rr, cc)] >> kvp.Key) & 1) == 0)
                                    {
                                        board.SetValue(rr, cc, val);
                                        SetBit(rr, cc, kvp.Key);
                                        changed = true;
                                        break;
                                    }
                                }
                                if (changed) break;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        private bool Backtrack()
        {
            int n = SudokuConstants.BoardSize;
            int minOptionsCount = n + 1;
            int chosenRow = -1;
            int chosenCol = -1;
            int highestDegree = -1;
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

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
                ApplyLogicStrategies();

                // Recurse
                if (Backtrack())
                    return true;

                // If recursion failed, restore the previous state
                RestoreState(clonedBoard, clonedRowMask, clonedColMask, clonedBoxMask);
            }

            return false;
        }

        // Helper method to calculate the degree of a cell
        private int CalculateDegree(int row, int col)
        {
            int degree = 0;
            int n = SudokuConstants.BoardSize;

            // Count the number of empty cells in the same row
            for (int c = 0; c < n; c++)
            {
                if (c != col && CountBits(board.GetOptions(row, c)) > 1)
                    degree++;
            }

            // Count the number of empty cells in the same column
            for (int r = 0; r < n; r++)
            {
                if (r != row && CountBits(board.GetOptions(r, col)) > 1)
                    degree++;
            }

            // Count the number of empty cells in the same box
            int boxIndex = GetBoxIndex(row, col);
            int subR = SudokuConstants.SubgridRows;
            int subC = SudokuConstants.SubgridCols;
            int startRow = (row / subR) * subR;
            int startCol = (col / subC) * subC;

            for (int r = startRow; r < startRow + subR; r++)
            {
                for (int c = startCol; c < startCol + subC; c++)
                {
                    if ((r != row || c != col) && CountBits(board.GetOptions(r, c)) > 1)
                        degree++;
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
