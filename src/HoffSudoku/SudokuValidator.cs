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
            // Get fixed values in the specified row
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                {
                    yield return options.First();
                }
            }
        }

        private static IEnumerable<int> GetSingleValuesInColumn(SudokuBoard board, int col)
        {
            // Get fixed values in the specified column
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                {
                    yield return options.First();
                }
            }
        }

        private static IEnumerable<int> GetSingleValuesInBox(SudokuBoard board, int startRow, int startCol)
        {
            // Get fixed values in the specified box
            int subgridRows = SudokuConstants.SubgridRows;
            int subgridCols = SudokuConstants.SubgridCols;

            for (int r = 0; r < subgridRows; r++)
            {
                for (int c = 0; c < subgridCols; c++)
                {
                    var options = board.GetOptions(startRow + r, startCol + c);
                    if (options.Count == 1)
                    {
                        yield return options.First();
                    }
                }
            }
        }

        private static bool HasDuplicate(IEnumerable<int> values)
        {
            // Check for duplicate values
            var set = new HashSet<int>();
            foreach (var value in values)
            {
                if (!set.Add(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
