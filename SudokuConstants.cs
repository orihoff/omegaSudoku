using System;

namespace omegaSudoku
{
    public static class SudokuConstants
    {
        public static int BoardSize { get; set; } = 9; // Default size
        public static int SubgridRows { get; private set; } // Rows in subgrid
        public static int SubgridCols { get; private set; } // Columns in subgrid
        public const int MinValue = 1; // Minimal value
        public const int Step = 1;    // Step between values

        static SudokuConstants()
        {
            CalculateSubgridDimensions();
        }

        private static void CalculateSubgridDimensions()
        {
            bool validDimensionsFound = false;

            for (int i = 1; i <= Math.Sqrt(BoardSize); i++)
            {
                if (BoardSize % i == 0)
                {
                    SubgridRows = i;
                    SubgridCols = BoardSize / i;
                    validDimensionsFound = true;
                }
            }

            if (!validDimensionsFound || SubgridRows * SubgridCols != BoardSize)
            {
                throw new InvalidOperationException($"Invalid board size: {BoardSize}x{BoardSize}. Unable to determine valid subgrid dimensions.");
            }
        }
    }
}
