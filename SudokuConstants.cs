using System;

namespace omegaSudoku
{
    public static class SudokuConstants
    {
        public static int BoardSize { get; set; } = 9; // Board size
        public static int MinValue { get; set; } = 1; // The minimum value in the table
        public static int MaxValue => MinValue + BoardSize - 1; // The maximum value
    }
}
