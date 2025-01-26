using omegaSudoku;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HoffSudoku
{
    public class SudokuBoard
    {
        private readonly Dictionary<(int, int), HashSet<int>> board;

        public SudokuBoard()
        {
            board = new Dictionary<(int, int), HashSet<int>>();
        }

        public void Initialize(string input)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            InputValidator.Validate(input);

            board.Clear();
            int index = 0;

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    int val = input[index++] - '0';

                    board[(r, c)] = val == 0
                        ? new HashSet<int>(CreatePossibilitiesList(minValue, boardSize, step))
                        : new HashSet<int> { val };
                }
            }
        }

        private HashSet<int> CreatePossibilitiesList(int minValue, int boardSize, int step)
        {
            var possibilities = new HashSet<int>();
            for (int i = minValue; i < minValue + boardSize * step; i += step)
                possibilities.Add(i);
            return possibilities;
        }

        public HashSet<int> GetOptions(int row, int col)
        {
            if (!board.ContainsKey((row, col)))
                throw new InvalidOperationException($"Cell ({row}, {col}) does not exist on the board.");
            return board[(row, col)];
        }

        public void ResetOptions(int row, int col)
        {
            if (!board.ContainsKey((row, col)))
                throw new InvalidOperationException($"Cannot reset options for cell ({row}, {col}) as it does not exist.");

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
                var options = GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options.First());
            }
            return used;
        }

        public IEnumerable<int> GetUsedInCol(int col)
        {
            var used = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options.First());
            }
            return used;
        }

        public IEnumerable<int> GetUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRow = row / SudokuConstants.SubgridRows * SudokuConstants.SubgridRows;
            int boxCol = col / SudokuConstants.SubgridCols * SudokuConstants.SubgridCols;

            for (int r = 0; r < SudokuConstants.SubgridRows; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                {
                    var options = GetOptions(boxRow + r, boxCol + c);
                    if (options.Count == 1)
                        used.Add(options.First());
                }
            }
            return used;
        }

        public HashSet<int> GetOptionsInRow(int row)
        {
            var result = new HashSet<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = GetOptions(row, col);
                if (options.Count == 1)
                    result.Add(options.First());
            }
            return result;
        }

        public HashSet<int> GetOptionsInColumn(int col)
        {
            var result = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = GetOptions(row, col);
                if (options.Count == 1)
                    result.Add(options.First());
            }
            return result;
        }

        public HashSet<int> GetOptionsInBox(int startRow, int startCol)
        {
            var result = new HashSet<int>();
            for (int r = 0; r < SudokuConstants.SubgridRows; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridCols; c++)
                {
                    var options = GetOptions(startRow + r, startCol + c);
                    if (options.Count == 1)
                        result.Add(options.First());
                }
            }
            return result;
        }

        private string GetDisplayValue(int value)
        {
            if (value >= 1 && value <= 9)
                return value.ToString();
            else if (value > 9)
                return ((char)(value + 48)).ToString();
            else
                return ".";
        }

        public void Print()
        {
            int boardSize = SudokuConstants.BoardSize;
            int subgridRows = SudokuConstants.SubgridRows;
            int subgridCols = SudokuConstants.SubgridCols;

            Console.Write("    ");
            for (int c = 0; c < boardSize; c++)
            {
                Console.Write($"{c + 1,3} ");
                if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                    Console.Write(" ");
            }
            Console.WriteLine();

            Console.WriteLine("    " + new string('-', boardSize * 4 + subgridCols - 1));

            for (int r = 0; r < boardSize; r++)
            {
                Console.Write($"{r + 1,2} |");
                for (int c = 0; c < boardSize; c++)
                {
                    var opts = GetOptions(r, c);
                    string display = opts.Count == 1 ? GetDisplayValue(opts.First()) : ".";
                    Console.Write($"{display,3} ");

                    if ((c + 1) % subgridCols == 0 && c != boardSize - 1)
                        Console.Write("|");
                }
                Console.WriteLine();

                if ((r + 1) % subgridRows == 0 && r != boardSize - 1)
                {
                    Console.Write("    ");
                    for (int i = 0; i < boardSize; i++)
                    {
                        Console.Write("----");
                        if ((i + 1) % subgridCols == 0 && i != boardSize - 1)
                            Console.Write("+");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("    " + new string('-', boardSize * 4 + subgridCols - 1));
        }

        public void SetValue(int row, int col, int value)
        {
            if (!board.ContainsKey((row, col)))
                throw new InvalidOperationException($"Cell ({row}, {col}) does not exist on the board.");

            if (value == 0)
            {
                board[(row, col)] = new HashSet<int>(CreatePossibilitiesList(SudokuConstants.MinValue, SudokuConstants.BoardSize, SudokuConstants.Step));
            }
            else
            {
                board[(row, col)] = new HashSet<int> { value };
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int boardSize = SudokuConstants.BoardSize;

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    var options = GetOptions(r, c);
                    if (options.Count == 1)
                    {
                        sb.Append(GetDisplayValue(options.First()));
                    }
                    else
                    {
                        sb.Append('0');
                    }
                }
                if (r < boardSize - 1)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
