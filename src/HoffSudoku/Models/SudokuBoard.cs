using System;
using System.Text;
using HoffSudoku.Validators;
using HoffSudoku.Exceptions;
using HoffSudoku.Helpers;

namespace HoffSudoku.Models
{
    public class SudokuBoard
    {
        private readonly int[,] boardOptions;

        /// <summary>
        /// Initializes an empty Sudoku board with the predefined board size.
        /// Throws an exception if the board size is not set.
        /// </summary>
        public SudokuBoard()
        {
            if (SudokuConstants.BoardSize <= 0)
                throw new InitializationException("Board size must be set before initializing the SudokuBoard.");

            int size = SudokuConstants.BoardSize;
            boardOptions = new int[size, size];
        }

        /// <summary>
        /// Creates a deep copy of the Sudoku board.
        /// </summary>
        private SudokuBoard(int[,] options)
        {
            if (SudokuConstants.BoardSize <= 0)
                throw new InitializationException("Board size must be set before cloning the SudokuBoard.");

            int size = SudokuConstants.BoardSize;
            boardOptions = new int[size, size];
            Array.Copy(options, boardOptions, options.Length);
        }

        /// <summary>
        /// Initializes the board with a given input string, setting values or possible options.
        /// </summary>
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

        /// <summary>
        /// Creates a bitmask representing all possible values for a cell.
        /// </summary>
        private int CreateBitmask(int minValue, int size, int step)
        {
            int mask = 0;
            for (int i = 0; i < size; i++)
            {
                mask |= (1 << i);
            }
            return mask;
        }

        /// <summary>
        /// Returns the bitmask options for a specific cell.
        /// </summary>
        public int GetOptions(int row, int col)
        {
            return boardOptions[row, col];
        }

        /// <summary>
        /// Sets a value in the board, updating the bitmask accordingly.
        /// </summary>
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

        /// <summary>
        /// Clears the options for a specific cell, setting it to 0.
        /// </summary>
        public void ClearOptions(int row, int col)
        {
            boardOptions[row, col] = 0;
        }

        /// <summary>
        /// Prints the Sudoku board in a readable format with grid separators.
        /// </summary>
        public void Print()
        {
            int size = SudokuConstants.BoardSize;
            int subRows = SudokuConstants.SubgridRows;
            int subCols = SudokuConstants.SubgridCols;

            Console.Write("    ");
            for (int c = 0; c < size; c++)
            {
                Console.Write($"{c + 1,3} ");
                if ((c + 1) % subCols == 0 && c != size - 1)
                    Console.Write(" ");
            }
            Console.WriteLine();

            Console.WriteLine("    " + new string('-', size * 4 + subCols - 1));

            for (int r = 0; r < size; r++)
            {
                Console.Write($"{r + 1,2} |");
                for (int c = 0; c < size; c++)
                {
                    int options = boardOptions[r, c];
                    string display = (SudokuHelper.CountBits(options) == 1) ? GetDisplayValue(options) : ".";
                    Console.Write($"{display,3} ");

                    if ((c + 1) % subCols == 0 && c != size - 1)
                        Console.Write("|");
                }
                Console.WriteLine();

                if ((r + 1) % subRows == 0 && r != size - 1)
                {
                    Console.Write("    ");
                    for (int i = 0; i < size; i++)
                    {
                        Console.Write("----");
                        if ((i + 1) % subCols == 0 && i != size - 1)
                            Console.Write("+");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("    " + new string('-', size * 4 + subCols - 1));
        }

        /// <summary>
        /// Converts a bitmask into a displayable Sudoku number.
        /// </summary>
        private string GetDisplayValue(int bitmask)
        {
            if (bitmask == 0)
                return ".";

            int value = (int)(Math.Log(bitmask, 2) + 1) + SudokuConstants.MinValue - 1;

            if (value <= 9)
            {
                return value.ToString();
            }
            else
            {
                return ((char)('0' + value)).ToString();
            }
        }

        /// <summary>
        /// Converts the Sudoku board into a string representation.
        /// </summary>
        public override string ToString()
        {
            if (SudokuConstants.BoardSize <= 0)
                throw new InitializationException("Board size must be set before converting SudokuBoard to string.");

            StringBuilder sb = new StringBuilder();
            int size = SudokuConstants.BoardSize;

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int options = boardOptions[r, c];
                    if (SudokuHelper.CountBits(options) == 1)
                    {
                        sb.Append(GetDisplayValue(options));
                    }
                    else
                    {
                        sb.Append('0');
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a deep copy of the current Sudoku board.
        /// </summary>
        public SudokuBoard Clone()
        {
            return new SudokuBoard(this.boardOptions);
        }
    }
}
