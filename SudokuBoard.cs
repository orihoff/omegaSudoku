using System;
using System.Collections.Generic;

namespace omegaSudoku
{
    public class SudokuBoard
    {
        private readonly Dictionary<(int, int), List<int>> board;

        public SudokuBoard()
        {
            board = new Dictionary<(int, int), List<int>>();
        }

        public void Initialize(string input)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            string validationMessage = InputValidator.Validate(input);
            if (!string.IsNullOrEmpty(validationMessage))
                throw new ArgumentException($"Invalid input: {validationMessage}");

            board.Clear();
            int index = 0;

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    int val = input[index++] - '0';

                    board[(r, c)] = val == 0
                        ? CreatePossibilitiesList(minValue, boardSize, step)
                        : new List<int> { val };
                }
            }
        }

        private List<int> CreatePossibilitiesList(int minValue, int boardSize, int step)
        {
            var possibilities = new List<int>();
            for (int i = minValue; i < minValue + boardSize * step; i += step)
                possibilities.Add(i);
            return possibilities;
        }

        public List<int> GetOptions(int row, int col) => board[(row, col)];

        public void ResetOptions(int row, int col)
        {
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;
            int boardSize = SudokuConstants.BoardSize;

            board[(row, col)].Clear();
            for (int i = minValue; i < minValue + boardSize * step; i += step)
            {
                board[(row, col)].Add(i);
            }
        }

        public IEnumerable<int> GetUsedInRow(int row)
        {
            var used = new HashSet<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board[(row, col)];
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        public IEnumerable<int> GetUsedInCol(int col)
        {
            var used = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board[(row, col)];
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        public IEnumerable<int> GetUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRow = (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridRows;
            int boxCol = (col / SudokuConstants.SubgridCols) * SudokuConstants.SubgridCols;

            for (int r = 0; r < SudokuConstants.SubgridRows; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                {
                    var options = board[(boxRow + r, boxCol + c)];
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
            }
            return used;
        }

        public List<int> GetOptionsInRow(int row)
        {
            var result = new List<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board[(row, col)];
                if (options.Count == 1)
                {
                    result.Add(options[0]);
                }
            }
            return result;
        }

        public List<int> GetOptionsInColumn(int col)
        {
            var result = new List<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board[(row, col)];
                if (options.Count == 1)
                {
                    result.Add(options[0]);
                }
            }
            return result;
        }

        public List<int> GetOptionsInBox(int startRow, int startCol)
        {
            var result = new List<int>();
            for (int r = 0; r < SudokuConstants.SubgridRows; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                {
                    var options = board[(startRow + r, startCol + c)];
                    if (options.Count == 1)
                    {
                        result.Add(options[0]);
                    }
                }
            }
            return result;
        }

        public void Print()
        {
            int boardSize = SudokuConstants.BoardSize; // Size of the board
            int subgridRows = SudokuConstants.SubgridRows; // Rows in each subgrid
            int subgridCols = SudokuConstants.SubgridCols; // Columns in each subgrid

            // Print column headers
            Console.Write("   "); // Padding for row numbers
            for (int c = 0; c < boardSize; c++)
            {
                Console.Write($" {c + 1} "); // Column numbers start from 1
                if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                    Console.Write(" ");
            }
            Console.WriteLine();

            // Print top border
            Console.WriteLine("   " + new string('-', boardSize * 3 + subgridCols - 1).Replace("-", "-"));

            for (int r = 0; r < boardSize; r++)
            {
                // Print row numbers
                Console.Write($"{r + 1,2} "); // Row numbers start from 1

                for (int c = 0; c < boardSize; c++)
                {
                    // Print cell value or a dot for empty
                    var opts = board[(r, c)];
                    Console.Write(opts.Count == 1 ? $" {opts[0]} " : " . ");

                    // Print vertical separators between subgrids
                    if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                        Console.Write("|");
                }

                Console.WriteLine(); // Move to the next line

                // Print horizontal separator between subgrids
                if ((r + 1) % subgridRows == 0 && r != boardSize - 1)
                {
                    Console.Write("   "); // Align with row numbers
                    for (int i = 0; i < boardSize; i++)
                    {
                        Console.Write("---"); // Horizontal line for each cell
                        if ((i + 1) % subgridCols == 0 && i != boardSize - 1)
                            Console.Write("+"); // Add '+' at subgrid intersections
                    }
                    Console.WriteLine();
                }
            }

            // Print bottom border
            Console.WriteLine("   " + new string('-', boardSize * 3 + subgridCols - 1));
        }

    }
}
