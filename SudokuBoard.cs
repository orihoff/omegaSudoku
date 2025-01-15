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
            board.Clear();
            int index = 0;

            for (int r = 0; r < SudokuConstants.BoardSize; r++)
            {
                for (int c = 0; c < SudokuConstants.BoardSize; c++)
                {
                    int val = input[index++] - '0';
                    board[(r, c)] = val == 0 ? CreatePossibilitiesList() : new List<int> { val };
                }
            }
        }

        private List<int> CreatePossibilitiesList()
        {
            var possibilities = new List<int>();
            for (int i = SudokuConstants.MinValue; i <= SudokuConstants.MaxValue; i++)
                possibilities.Add(i);
            return possibilities;
        }

        public List<int> GetOptions(int row, int col) => board[(row, col)];

        public void Print()
        {
            for (int r = 0; r < SudokuConstants.BoardSize; r++)
            {
                for (int c = 0; c < SudokuConstants.BoardSize; c++)
                {
                    var opts = board[(r, c)];
                    Console.Write(opts.Count == 1 ? $"{opts[0]} " : ". ");
                }
                Console.WriteLine();
            }
        }

        public HashSet<int> GetUsedInRow(int row)
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

        public HashSet<int> GetUsedInCol(int col)
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

        public HashSet<int> GetUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRowStart = (row / SudokuConstants.SubgridHeight) * SudokuConstants.SubgridHeight;
            int boxColStart = (col / SudokuConstants.SubgridWidth) * SudokuConstants.SubgridWidth;

            for (int r = 0; r < SudokuConstants.SubgridHeight; r++)
            {
                for (int c = 0; c < SudokuConstants.SubgridWidth; c++)
                {
                    var options = board[(boxRowStart + r, boxColStart + c)];
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
            }
            return used;
        }
    }
}
