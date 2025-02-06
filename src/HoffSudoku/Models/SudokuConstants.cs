using System;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Models
{
    public static class SudokuConstants
    {
        /// <summary>
        /// Stores global Sudoku settings, including board size and subgrid dimensions.
        /// These values must be set before initializing the Sudoku board.
        /// </summary>
        public static int BoardSize { get; private set; } = 0;
        public static int SubgridRows { get; private set; }
        public static int SubgridCols { get; private set; }
        public const int MinValue = 1;
        public const int Step = 1;//right now step is unused but it's a nice feature that could be changed in the future for the sake of generics

        /// <summary>
        /// Sets the board size and calculates the subgrid dimensions.
        /// Throws an exception if the board size is not a perfect square.
        /// </summary>
        public static void SetBoardSize(int size)
        {
            double sqrt = Math.Sqrt(size);

            if (sqrt % 1 != 0)
            {
                throw new InvalidInputException(
                    $"Invalid board size: {size}x{size}. Board size must have an integer square root."
                );
            }

            BoardSize = size;
            SubgridRows = SubgridCols = (int)sqrt;
        }
    }
}
