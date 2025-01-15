using System;

namespace omegaSudoku
{
    public static class InputValidator
    {
        public static string Validate(string input)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int maxValue = minValue + (boardSize - 1) * SudokuConstants.Step;

            if (string.IsNullOrEmpty(input))
                return "Input is null or empty.";

            if (input.Length != boardSize * boardSize)
                return $"Input length is incorrect. Expected {boardSize * boardSize} characters, but got {input.Length}.";

            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                    return $"Input contains a non-digit character: '{c}'.";

                int val = c - '0';
                if (val != 0 && (val < minValue || val > maxValue || (val - minValue) % SudokuConstants.Step != 0))
                    return $"Input contains a number out of range or invalid step: {val}. Expected numbers between {minValue} and {maxValue}, or '0' to represent an empty cell.";
            }

            return string.Empty;
        }
    }
}
