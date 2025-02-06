using System;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Solvers.Heuristics
{
    /// <summary>
    /// Applies the Hidden Singles strategy to the Sudoku board.
    /// </summary>
    public static class HiddenSingles
    {
        /// <summary>
        /// Applies Hidden Singles for rows, columns, and boxes.
        /// </summary>
        public static bool ApplyHiddenSingles(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            bool changed = false;
            changed |= ApplyHiddenSinglesForRows(board, rowMask, colMask, boxMask);
            changed |= ApplyHiddenSinglesForCols(board, rowMask, colMask, boxMask);
            changed |= ApplyHiddenSinglesForBoxes(board, rowMask, colMask, boxMask);
            return changed;
        }

        /// <summary>
        /// Applies Hidden Singles for each row.
        /// </summary>
        private static bool ApplyHiddenSinglesForRows(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
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
                        // Find the cell that can take this value.
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
                                SetBit(r, c, bitIndex: kvp.Key, rowMask, colMask, boxMask, GetBoxIndex(r, c));
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// Applies Hidden Singles for each column.
        /// </summary>
        private static bool ApplyHiddenSinglesForCols(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
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
                        // Find the cell that can take this value.
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
                                SetBit(r, c, bitIndex: kvp.Key, rowMask, colMask, boxMask, GetBoxIndex(r, c));
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// Applies Hidden Singles for each box (subgrid).
        /// </summary>
        private static bool ApplyHiddenSinglesForBoxes(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
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
                            // Find the cell that can take this value.
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
                                        SetBit(rr, cc, bitIndex: kvp.Key, rowMask, colMask, boxMask, GetBoxIndex(rr, cc));
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

        /// <summary>
        /// Returns the box index based on the row and column.
        /// </summary>
        private static int GetBoxIndex(int row, int col)
        {
            return (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridCols
                   + (col / SudokuConstants.SubgridCols);
        }

        /// <summary>Updates the masks to mark that the value is set in the cell.</summary>
        private static void SetBit(int row, int col, int bitIndex, int[] rowMask, int[] colMask, int[] boxMask, int boxIndex)
        {
            rowMask[row] |= (1 << bitIndex);
            colMask[col] |= (1 << bitIndex);
            boxMask[boxIndex] |= (1 << bitIndex);
        }

        /// <summary>Counts the number of active bits in the given number.</summary>
        private static int CountBits(int n)
        {
            return BitOperations.PopCount((uint)n);
        }
    }
}
