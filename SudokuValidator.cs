using System;
using System.Collections.Generic;
using System.Linq;

namespace omegaSudoku
{
    public static class SudokuValidator
    {
        public static string ValidateInitialBoard(SudokuBoard board)
        {
            int boardSize = SudokuConstants.BoardSize;

            for (int row = 0; row < boardSize; row++)
            {
                var filledValues = board.GetOptionsInRow(row)
                                        .Where(val => val != 0)
                                        .ToList();

                if (HasDuplicate(filledValues))
                {
                    return $"Invalid Sudoku: duplicate value found in row {row + 1}";
                }
            }

            for (int col = 0; col < boardSize; col++)
            {
                var filledValues = board.GetOptionsInColumn(col)
                                        .Where(val => val != 0)
                                        .ToList();

                if (HasDuplicate(filledValues))
                {
                    return $"Invalid Sudoku: duplicate value found in column {col + 1}";
                }
            }

            int subgridRows = SudokuConstants.SubgridRows;
            int subgridCols = SudokuConstants.SubgridCols;

            for (int boxRow = 0; boxRow < boardSize; boxRow += subgridRows)
            {
                for (int boxCol = 0; boxCol < boardSize; boxCol += subgridCols)
                {
                    var filledValues = board.GetOptionsInBox(boxRow, boxCol)
                                            .Where(val => val != 0)
                                            .ToList();

                    if (HasDuplicate(filledValues))
                    {
                        return $"Invalid Sudoku: duplicate value found in box starting at ({boxRow + 1}, {boxCol + 1})";
                    }
                }
            }

            return string.Empty;
        }

        private static bool HasDuplicate(IEnumerable<int> values)
        {
            var set = new HashSet<int>();
            foreach (int val in values)
            {
                if (set.Contains(val))
                {
                    return true;
                }
                set.Add(val);
            }
            return false;
        }
    }
}
