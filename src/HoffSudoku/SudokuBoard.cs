using omegaSudoku;
using System;
using System.Text;

namespace HoffSudoku
{
    public class SudokuBoard
    {
        private readonly int[,] boardOptions;

        public SudokuBoard()
        {
            int size = SudokuConstants.BoardSize;
            boardOptions = new int[size, size];
        }

        // Copy constructor for cloning
        private SudokuBoard(int[,] options)
        {
            int size = SudokuConstants.BoardSize;
            boardOptions = new int[size, size];
            Array.Copy(options, boardOptions, options.Length);
        }

        public void Initialize(string input)
        {
            int size = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            InputValidator.Validate(input);

            int index = 0;

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int val = input[index++] - '0';
                    if (val == 0)
                    {
                        boardOptions[r, c] = CreateBitmask(minValue, size, step);
                    }
                    else
                    {
                        boardOptions[r, c] = 1 << ((val - minValue) / step);
                    }
                }
            }
        }

        private int CreateBitmask(int minValue, int size, int step)
        {
            int mask = 0;
            for (int i = 0; i < size; i++)
            {
                mask |= (1 << i);
            }
            return mask;
        }

        public int GetOptions(int row, int col)
        {
            return boardOptions[row, col];
        }

        public void SetValue(int row, int col, int value)
        {
            if (value == 0)
            {
                boardOptions[row, col] = CreateBitmask(SudokuConstants.MinValue, SudokuConstants.BoardSize, SudokuConstants.Step);
            }
            else
            {
                boardOptions[row, col] = 1 << ((value - SudokuConstants.MinValue) / SudokuConstants.Step);
            }
        }

        public void ClearOptions(int row, int col)
        {
            boardOptions[row, col] = 0;
        }

        public void Print()
        {
            int size = SudokuConstants.BoardSize;
            int subRows = SudokuConstants.SubgridRows;
            int subCols = SudokuConstants.SubgridCols;

            // Print column headers
            Console.Write("    ");
            for (int c = 0; c < size; c++)
            {
                Console.Write($"{c + 1,3} ");
                if ((c + 1) % subCols == 0 && c != size - 1)
                    Console.Write(" ");
            }
            Console.WriteLine();

            // Print separator
            Console.WriteLine("   " + new string('-', size * 4 + subCols - 1));

            for (int r = 0; r < size; r++)
            {
                Console.Write($"{r + 1,2} |");
                for (int c = 0; c < size; c++)
                {
                    int options = boardOptions[r, c];
                    string display = (CountBits(options) == 1) ? GetDisplayValue(options) : ".";
                    Console.Write($"{display,3} ");

                    if ((c + 1) % subCols == 0 && c != size - 1)
                        Console.Write("|");
                }
                Console.WriteLine();

                if ((r + 1) % subRows == 0 && r != size - 1)
                {
                    Console.Write("   ");
                    for (int i = 0; i < size; i++)
                    {
                        Console.Write("----");
                        if ((i + 1) % subCols == 0 && i != size - 1)
                            Console.Write("+");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("   " + new string('-', size * 4 + subCols - 1));
        }

        private int CountBits(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }

        private string GetDisplayValue(int bitmask)
        {
            if (bitmask == 0)
                return ".";
            int value = (int)(Math.Log(bitmask, 2) + 1) + SudokuConstants.MinValue - 1;
            return value.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int size = SudokuConstants.BoardSize;

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int options = boardOptions[r, c];
                    if (CountBits(options) == 1)
                    {
                        sb.Append(GetDisplayValue(options));
                    }
                    else
                    {
                        sb.Append('0');
                    }
                }
                if (r < size - 1)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        // Clone method to create a deep copy of the board
        public SudokuBoard Clone()
        {
            return new SudokuBoard(this.boardOptions);
        }
    }
}
