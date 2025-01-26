﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace HoffSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        // Using int arrays for row, column, and box checks.
        private int[] rowMask;
        private int[] colMask;
        private int[] boxMask;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;

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
            while (ApplyLogicStrategies()) { }

            // Start backtracking
            Stopwatch sw = Stopwatch.StartNew();
            bool solved = Backtrack();
            sw.Stop();

            Console.WriteLine($"Time taken to solve: {sw.ElapsedMilliseconds} ms");
            return solved;
        }

        private bool ApplyLogicStrategies()
        {
            bool changed = false;

            changed |= ApplyNakedSingles();
            changed |= ApplyHiddenSingles();

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
                    if (CountBits(options) == 1) continue;

                    // Count valid options based on masks
                    List<int> validValues = new List<int>();
                    for (int i = 0; i < n; i++)
                    {
                        if (((options >> i) & 1) == 1 &&
                            ((rowMask[r] >> i) & 1) == 0 &&
                            ((colMask[c] >> i) & 1) == 0 &&
                            ((boxMask[GetBoxIndex(r, c)] >> i) & 1) == 0)
                        {
                            validValues.Add(i);
                        }
                    }

                    if (validValues.Count == 1)
                    {
                        int val = validValues[0] + minVal;
                        board.SetValue(r, c, val);
                        SetBit(r, c, validValues[0]);
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
            int minVal = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            // Find the cell with the fewest valid options
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (CountBits(options) == 1) continue;

                    int validCount = 0;
                    for (int i = 0; i < n; i++)
                    {
                        if (((options >> i) & 1) == 1 &&
                            ((rowMask[r] >> i) & 1) == 0 &&
                            ((colMask[c] >> i) & 1) == 0 &&
                            ((boxMask[GetBoxIndex(r, c)] >> i) & 1) == 0)
                        {
                            validCount++;
                        }
                    }

                    if (validCount == 0)
                        return false;

                    if (validCount < minOptionsCount)
                    {
                        minOptionsCount = validCount;
                        chosenRow = r;
                        chosenCol = c;
                    }
                }
            }

            // If no cell is chosen, the puzzle is solved
            if (chosenRow == -1 && chosenCol == -1)
                return true;

            int cellOptions = board.GetOptions(chosenRow, chosenCol);
            for (int i = 0; i < SudokuConstants.BoardSize; i++)
            {
                if (((cellOptions >> i) & 1) == 0)
                    continue;

                if (((rowMask[chosenRow] >> i) & 1) == 1 ||
                    ((colMask[chosenCol] >> i) & 1) == 1 ||
                    ((boxMask[GetBoxIndex(chosenRow, chosenCol)] >> i) & 1) == 1)
                    continue;

                // Assign the value
                board.SetValue(chosenRow, chosenCol, i + minVal);
                SetBit(chosenRow, chosenCol, i);

                // Recurse
                if (Backtrack())
                    return true;

                // Undo the assignment
                board.SetValue(chosenRow, chosenCol, 0);
                ClearBit(chosenRow, chosenCol, i);
            }

            return false;
        }

        private int CountBits(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }
    }
}
