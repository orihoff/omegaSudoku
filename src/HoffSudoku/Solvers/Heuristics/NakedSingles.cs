using System;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Helpers;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Solvers.Heuristics
{
    /// <summary>
    /// Identifies and applies the Naked Singles technique.
    /// If a cell has only one possible candidate, it is set as the final value.
    /// </summary>
    public static class NakedSingles
    {
        public static bool ApplyNakedSingles(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            bool changed = false;
            int n = SudokuConstants.BoardSize;
            int minVal = SudokuConstants.MinValue;

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1) continue; // Skip cells with a fixed value

                    // Calculate box index once per cell
                    int boxIndex = SudokuHelper.GetBoxIndex(r, c);

                    // Efficiently identify single candidates using bitwise operations
                    int possibleValues = options & ~(rowMask[r] | colMask[c] | boxMask[boxIndex]);

                    if (SudokuHelper.CountBits(possibleValues) == 1)
                    {
                        // Extract the single valid value
                        int singleBit = possibleValues & -possibleValues; // Isolate the lowest set bit
                        int valIndex = BitOperations.Log2((uint)singleBit);
                        int val = valIndex + minVal;

                        // Set the value and update masks
                        board.SetValue(r, c, val);
                        SudokuHelper.SetBit(r, c, valIndex, rowMask, colMask, boxMask, boxIndex);
                        changed = true;
                    }
                }
            }

            return changed;
        }
    }
}
