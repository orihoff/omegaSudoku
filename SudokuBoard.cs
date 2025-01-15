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

        public void Print()
        {
            int boardSize = SudokuConstants.BoardSize;

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    var opts = board[(r, c)];
                    Console.Write(opts.Count == 1 ? $"{opts[0]} " : ". ");
                }
                Console.WriteLine();
            }
        }
    }
}
