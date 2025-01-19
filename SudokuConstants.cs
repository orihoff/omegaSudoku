using System;

namespace omegaSudoku
{
    public static class SudokuConstants
    {
        public static int BoardSize { get; set; } = 5; // Default size
        public static int SubgridRows { get; private set; } // Rows in subgrid
        public static int SubgridCols { get; private set; } // Columns in subgrid
        public const int MinValue = 1; // Minimal value
        public const int Step = 1;    // Step between values

        static SudokuConstants()
        {
            ValidateBoardSizeAndCalculateSubgridDimensions();
        }

        private static void ValidateBoardSizeAndCalculateSubgridDimensions()
        {
            double sqrt = Math.Sqrt(BoardSize);

            if (sqrt % 1 != 0) // Check if sqrt is not an integer
            {
                Console.WriteLine($"Invalid board size: {BoardSize}x{BoardSize}. Board size must have an integer square root. Defaulting to 9x9.");
                BoardSize = 9; // Default value
                sqrt = Math.Sqrt(BoardSize);
            }

            SubgridRows = SubgridCols = (int)sqrt;
        }
    }
}
