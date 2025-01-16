using System;
using System.Collections.Generic;

namespace omegaSudoku
{
    public static class SudokuValidator
    {
        public static string Validate(SudokuBoard board)
        {
            for (int i = 0; i < SudokuConstants.BoardSize; i++)
            {
                if (HasDuplicates(board.GetUsedInRow(i)))
                    return $"Invalid Sudoku: Duplicates in row {i}.";
                if (HasDuplicates(board.GetUsedInCol(i)))
                    return $"Invalid Sudoku: Duplicates in column {i}.";
            }

            for (int row = 0; row < SudokuConstants.BoardSize; row += SudokuConstants.SubgridRows)
            {
                for (int col = 0; col < SudokuConstants.BoardSize; col += SudokuConstants.SubgridCols)
                {
                    if (HasDuplicates(board.GetUsedInBox(row, col)))
                        return $"Invalid Sudoku: Duplicates in subgrid starting at ({row + 1}, {col + 1}).";
                }
            }

            return string.Empty;
        }

        private static bool HasDuplicates(IEnumerable<int> values)
        {
            var seen = new HashSet<int>();
            foreach (var val in values)
            {
                if (val != 0 && seen.Contains(val))
                    return true;
                seen.Add(val);
            }
            return false;
        }
    }
}
