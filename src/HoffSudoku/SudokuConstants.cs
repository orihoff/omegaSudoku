namespace HoffSudoku
{
    public static class SudokuConstants
    {
        public static int BoardSize { get; private set; } = 0; 
        public static int SubgridRows { get; private set; }
        public static int SubgridCols { get; private set; }
        public const int MinValue = 1;
        public const int Step = 1;

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
