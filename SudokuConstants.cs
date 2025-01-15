using System;

namespace omegaSudoku
{
    public static class SudokuConstants
    {
        public static int BoardSize { get; set; } = 4; // Board size
        public static int MinValue { get; set; } = 1; // The minimum value in the table
        public static int MaxValue => MinValue + BoardSize - 1; // The maximum value

        // Mini-square dimensions (calculated automatically based on board size)
        public static int SubgridHeight { get; private set; }
        public static int SubgridWidth { get; private set; }

        // Static constructor to calculate subgrid dimensions
        static SudokuConstants()
        {
            CalculateSubgridDimensions();
        }

        private static void CalculateSubgridDimensions()
        {
            // Finding factors of BoardSize to determine valid subgrid dimensions
            for (int i = 1; i <= Math.Sqrt(BoardSize); i++)
            {
                if (BoardSize % i == 0)
                {
                    SubgridHeight = i;
                    SubgridWidth = BoardSize / i;
                }
            }

            if (SubgridHeight * SubgridWidth != BoardSize)
            {
                throw new InvalidOperationException($"Invalid board size: {BoardSize}. Cannot determine valid subgrid dimensions.");
            }
        }
    }
}
