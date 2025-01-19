using System;
using System.Collections.Generic;
using omegaSudoku.Exceptions; // Import custom exceptions

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

            // Validate input - will throw InvalidInputException if invalid
            InputValidator.Validate(input);

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

        public List<int> GetOptions(int row, int col)
        {
            if (!board.ContainsKey((row, col)))
                throw new System.InvalidOperationException($"Cell ({row}, {col}) does not exist on the board.");
            return board[(row, col)];
        }

        public void ResetOptions(int row, int col)
        {
            if (!board.ContainsKey((row, col)))
                throw new System.InvalidOperationException($"Cannot reset options for cell ({row}, {col}) as it does not exist.");

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
            try
            {
                var used = new HashSet<int>();
                for (int col = 0; col < SudokuConstants.BoardSize; col++)
                {
                    var options = GetOptions(row, col);
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
                return used;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving used numbers in row {row}: {ex.Message}");
            }
        }

        public IEnumerable<int> GetUsedInCol(int col)
        {
            try
            {
                var used = new HashSet<int>();
                for (int row = 0; row < SudokuConstants.BoardSize; row++)
                {
                    var options = GetOptions(row, col);
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
                return used;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving used numbers in column {col}: {ex.Message}");
            }
        }

        public IEnumerable<int> GetUsedInBox(int row, int col)
        {
            try
            {
                var used = new HashSet<int>();
                int boxRow = (row / SudokuConstants.SubgridRows) * SudokuConstants.SubgridRows;
                int boxCol = (col / SudokuConstants.SubgridCols) * SudokuConstants.SubgridCols;

                for (int r = 0; r < SudokuConstants.SubgridRows; r++)
                {
                    for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                    {
                        var options = GetOptions(boxRow + r, boxCol + c);
                        if (options.Count == 1)
                            used.Add(options[0]);
                    }
                }
                return used;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving used numbers in box containing ({row}, {col}): {ex.Message}");
            }
        }

        public List<int> GetOptionsInRow(int row)
        {
            try
            {
                var result = new List<int>();
                for (int col = 0; col < SudokuConstants.BoardSize; col++)
                {
                    var options = GetOptions(row, col);
                    if (options.Count == 1)
                        result.Add(options[0]);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving options in row {row}: {ex.Message}");
            }
        }

        public List<int> GetOptionsInColumn(int col)
        {
            try
            {
                var result = new List<int>();
                for (int row = 0; row < SudokuConstants.BoardSize; row++)
                {
                    var options = GetOptions(row, col);
                    if (options.Count == 1)
                        result.Add(options[0]);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving options in column {col}: {ex.Message}");
            }
        }

        public List<int> GetOptionsInBox(int startRow, int startCol)
        {
            try
            {
                var result = new List<int>();
                for (int r = 0; r < SudokuConstants.SubgridRows; r++)
                {
                    for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                    {
                        var options = GetOptions(startRow + r, startCol + c);
                        if (options.Count == 1)
                            result.Add(options[0]);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while retrieving options in box starting at ({startRow}, {startCol}): {ex.Message}");
            }
        }

        public void Print()
        {
            try
            {
                int boardSize = SudokuConstants.BoardSize;
                int subgridRows = SudokuConstants.SubgridRows;
                int subgridCols = SudokuConstants.SubgridCols;

                Console.Write("   ");
                for (int c = 0; c < boardSize; c++)
                {
                    Console.Write($" {c + 1} ");
                    if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                        Console.Write(" ");
                }
                Console.WriteLine();

                Console.WriteLine("   " + new string('-', boardSize * 3 + subgridCols - 1));

                for (int r = 0; r < boardSize; r++)
                {
                    Console.Write($"{r + 1,2} ");
                    for (int c = 0; c < boardSize; c++)
                    {
                        var opts = GetOptions(r, c);
                        Console.Write(opts.Count == 1 ? $" {opts[0]} " : " . ");

                        if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                            Console.Write("|");
                    }
                    Console.WriteLine();

                    if ((r + 1) % subgridRows == 0 && r != boardSize - 1)
                    {
                        Console.Write("   ");
                        for (int i = 0; i < boardSize; i++)
                        {
                            Console.Write("---");
                            if ((i + 1) % subgridCols == 0 && i != boardSize - 1)
                                Console.Write("+");
                        }
                        Console.WriteLine();
                    }
                }

                Console.WriteLine("   " + new string('-', boardSize * 3 + subgridCols - 1));
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Error while printing the board: {ex.Message}");
            }
        }
    }
}
