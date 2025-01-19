using System;
using omegaSudoku.Exceptions; // Import custom exceptions

namespace omegaSudoku
{
    public static class InputValidator
    {
        public static void Validate(string input)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int maxValue = minValue + (boardSize - 1) * SudokuConstants.Step;

            if (string.IsNullOrEmpty(input))
                throw new InvalidInputException("Input is null or empty.");

            if (input.Length != boardSize * boardSize)
                throw new InvalidInputException($"Input length is incorrect. Expected {boardSize * boardSize} characters, but got {input.Length}.");

            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                    throw new InvalidInputException($"Input contains a non-digit character: '{c}'.");

                int val = c - '0';
                if (val != 0 && (val < minValue || val > maxValue || (val - minValue) % SudokuConstants.Step != 0))
                    throw new InvalidInputException($"Input contains a number out of range or invalid step: {val}. Expected numbers between {minValue} and {maxValue}, or '0' to represent an empty cell.");
            }
        }
    }
}
