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

            for (int r = 0; r < 9; r++) 
            {
                for (int c = 0; c < 9; c++)
                {
                    int val = input[index++] - '0';
                    board[(r, c)] = val == 0 ? new List<int>() : new List<int> { val };
                }
            }
        }

        public void Print()
        {
            for (int r = 0; r < 9; r++) 
            {
                for (int c = 0; c < 9; c++)
                {
                    var opts = board[(r, c)];
                    Console.Write(opts.Count == 1 ? $"{opts[0]} " : ". ");
                }
                Console.WriteLine();
            }
        }
    }
}
