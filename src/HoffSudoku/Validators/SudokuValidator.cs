using System;
using System.Collections.Generic;
using System.Linq;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Validators
{
    public static class SudokuValidator
    {
        /// <summary>
        /// Validates the initial Sudoku board by checking for duplicate values
        /// in rows, columns, and boxes. Throws an exception if invalid.
        /// </summary>
        public static void ValidateInitialBoard(SudokuBoard board)
        {
            int boardSize = SudokuConstants.BoardSize;

            // Check each row for duplicate values
            for (int row = 0; row < boardSize; row++)
            {
                var values = GetSingleValuesInRow(board, row);
                if (HasDuplicate(values))
                {
                    throw new InvalidBoardException($"Invalid Sudoku: duplicate value found in row {row + 1}");
                }
            }

            // Check each column for duplicate values
            for (int col = 0; col < boardSize; col++)
            {
                var values = GetSingleValuesInColumn(board, col);
                if (HasDuplicate(values))
                {
                    throw new InvalidBoardException($"Invalid Sudoku: duplicate value found in column {col + 1}");
                }
            }

            // Check each subgrid (box) for duplicate values
            int subgridRows = SudokuConstants.SubgridRows;
            int subgridCols = SudokuConstants.SubgridCols;

            for (int boxRow = 0; boxRow < boardSize; boxRow += subgridRows)
            {
                for (int boxCol = 0; boxCol < boardSize; boxCol += subgridCols)
                {
                    var values = GetSingleValuesInBox(board, boxRow, boxCol);
                    if (HasDuplicate(values))
                    {
                        throw new InvalidBoardException($"Invalid Sudoku: duplicate value found in box starting at ({boxRow + 1}, {boxCol + 1})");
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all fixed values in the specified row.
        /// </summary>
        private static IEnumerable<int> GetSingleValuesInRow(SudokuBoard board, int row)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int col = 0; col < boardSize; col++)
            {
                int options = board.GetOptions(row, col);
                if (CountBits(options) == 1) // Only return cells with a single possible value
                {
                    yield return BitmaskToValue(options, minValue, step);
                }
            }
        }

        /// <summary>
        /// Retrieves all fixed values in the specified column.
        /// </summary>
        private static IEnumerable<int> GetSingleValuesInColumn(SudokuBoard board, int col)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int row = 0; row < boardSize; row++)
            {
                int options = board.GetOptions(row, col);
                if (CountBits(options) == 1) // Only return cells with a single possible value
                {
                    yield return BitmaskToValue(options, minValue, step);
                }
            }
        }

        /// <summary>
        /// Retrieves all fixed values in the specified subgrid (box).
        /// </summary>
        private static IEnumerable<int> GetSingleValuesInBox(SudokuBoard board, int startRow, int startCol)
        {
            int subgridRows = SudokuConstants.SubgridRows;
            int subgridCols = SudokuConstants.SubgridCols;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int r = 0; r < subgridRows; r++)
            {
                for (int c = 0; c < subgridCols; c++)
                {
                    int currentRow = startRow + r;
                    int currentCol = startCol + c;
                    int options = board.GetOptions(currentRow, currentCol);
                    if (CountBits(options) == 1) // Only return cells with a single possible value
                    {
                        yield return BitmaskToValue(options, minValue, step);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a bitmask representation to its corresponding Sudoku value.
        /// </summary>
        private static int BitmaskToValue(int bitmask, int minValue, int step)
        {
            int position = 0;
            while (bitmask > 1) // Find the index of the set bit
            {
                bitmask >>= 1;
                position++;
            }
            return minValue + (position * step);
        }

        /// <summary>
        /// Counts the number of set bits in an integer (that is the number of possible values in a cell).
        /// </summary>
        private static int CountBits(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }

        /// <summary>
        /// Checks if a collection contains duplicate values.
        /// Returns true if a duplicate is found.
        /// </summary>
        private static bool HasDuplicate(IEnumerable<int> values)
        {
            var seen = new HashSet<int>();
            foreach (var value in values)
            {
                if (!seen.Add(value)) // If value is already in the set, it's a duplicate
                {
                    return true;
                }
            }
            return false;
        }
    }
}
