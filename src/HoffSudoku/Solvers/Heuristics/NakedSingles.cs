using System;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Solvers.Heuristics
{
    public static class NakedSingles
    {
        public static bool ApplyNakedSingles(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
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
                        SetBit(r, c, bitIndex: valIndex, rowMask, colMask, boxMask, boxIndex);
                        changed = true;
                    }
                }
            }

            return changed;
        }

        private static int GetBoxIndex(int row, int col)
        {
            return (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridCols
                   + (col / SudokuConstants.SubgridCols);
        }

        private static void SetBit(int row, int col, int bitIndex, int[] rowMask, int[] colMask, int[] boxMask, int boxIndex)
        {
            rowMask[row] |= (1 << bitIndex);
            colMask[col] |= (1 << bitIndex);
            boxMask[boxIndex] |= (1 << bitIndex);
        }

        // It will use the CPU itself to quickly calculate the number of bits.
        private static int CountBits(int n)
        {
            return BitOperations.PopCount((uint)n);
        }
    }
}
