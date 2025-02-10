using HoffSudoku.Models;
using System.Numerics;

namespace HoffSudoku.Helpers
{
    /// <summary>
    /// Provides helper functions for Sudoku operations.
    /// </summary>
    public static class SudokuHelper
    {
        /// <summary>
        /// Computes the subgrid index using division.
        /// </summary>
        public static int GetBoxIndex(int row, int col)
        {
            return (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridCols
                   + (col / SudokuConstants.SubgridCols);
        }

        /// <summary>
        /// Counts the number of set bits in an integer.
        /// </summary>
        public static int CountBits(int n)
        {   // Use CPU's popcount for speed.
            return BitOperations.PopCount((uint)n);
        }

        /// <summary>
        /// Sets the corresponding bit in the row, column, and box masks.
        /// </summary>
        public static void SetBit(int row, int col, int bitIndex, int[] rowMask, int[] colMask, int[] boxMask, int boxIndex)
        {
            rowMask[row] |= (1 << bitIndex); // Turns on the bit at bitIndex to mark the number as present in the row.
            colMask[col] |= (1 << bitIndex);// same for colum
            boxMask[boxIndex] |= (1 << bitIndex);// same for box
        }
    }
}
