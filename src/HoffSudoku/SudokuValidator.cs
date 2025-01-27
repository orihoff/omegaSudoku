using System;
using System.Collections.Generic;
using System.Linq;

namespace HoffSudoku
{
    public static class SudokuValidator
    {
        public static void ValidateInitialBoard(SudokuBoard board)
        {
            int boardSize = SudokuConstants.BoardSize;

            // Check rows for duplicates
            for (int row = 0; row < boardSize; row++)
            {
                var values = GetSingleValuesInRow(board, row);
                if (HasDuplicate(values))
                {
                    throw new InvalidBoardException($"Invalid Sudoku: duplicate value found in row {row + 1}");
                }
            }

            // Check columns for duplicates
            for (int col = 0; col < boardSize; col++)
            {
                var values = GetSingleValuesInColumn(board, col);
                if (HasDuplicate(values))
                {
                    throw new InvalidBoardException($"Invalid Sudoku: duplicate value found in column {col + 1}");
                }
            }

            // Check boxes for duplicates
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

        private static IEnumerable<int> GetSingleValuesInRow(SudokuBoard board, int row)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int col = 0; col < boardSize; col++)
            {
                int options = board.GetOptions(row, col);
                if (CountBits(options) == 1)
                {
                    yield return BitmaskToValue(options, minValue, step);
                }
            }
        }

        private static IEnumerable<int> GetSingleValuesInColumn(SudokuBoard board, int col)
        {
            int boardSize = SudokuConstants.BoardSize;
            int minValue = SudokuConstants.MinValue;
            int step = SudokuConstants.Step;

            for (int row = 0; row < boardSize; row++)
            {
                int options = board.GetOptions(row, col);
                if (CountBits(options) == 1)
                {
                    yield return BitmaskToValue(options, minValue, step);
                }
            }
        }

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
                    if (CountBits(options) == 1)
                    {
                        yield return BitmaskToValue(options, minValue, step);
                    }
                }
            }
        }

        private static int BitmaskToValue(int bitmask, int minValue, int step)
        {
            // Find the position of the single set bit
            int position = 0;
            while (bitmask > 1)
            {
                bitmask >>= 1;
                position++;
            }
            return minValue + (position * step);
        }

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

        private static bool HasDuplicate(IEnumerable<int> values)
        {
            var seen = new HashSet<int>();
            foreach (var value in values)
            {
                if (!seen.Add(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
