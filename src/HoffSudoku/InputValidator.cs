using HoffSudoku;
using System;

namespace omegaSudoku
{
    public static class InputValidator
    {
        public static void Validate(string input)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int maxValue = minValue + (boardSize - 1) * SudokuConstants.Step;

            // Determine valid character range based on board size
            char minChar = (char)(minValue + '0'); // Smallest valid character
            char maxChar = (char)(maxValue + '0'); // Largest valid character

            // Check for null or empty input
            if (string.IsNullOrEmpty(input))
                throw new InvalidInputException("Input is null or empty.");

            // Validate input length
            if (input.Length != boardSize * boardSize)
                throw new InvalidInputException($"Input length is incorrect. Expected {boardSize * boardSize} characters, but got {input.Length}.");

            // Check each character in the input
            foreach (char c in input)
            {
                if (c != '0' && (c < minChar || c > maxChar))
                    throw new InvalidInputException($"Input contains an invalid character: '{c}'. Expected characters between '{minChar}' and '{maxChar}', or '0' for an empty cell.");
            }
        }
    }
}
