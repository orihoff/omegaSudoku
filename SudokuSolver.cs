﻿using System;
using System.Collections.Generic;

namespace omegaSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
        }

        public bool Solve()
        {
            return Backtrack(0, 0);
        }

        private bool Backtrack(int row, int col)
        {
            // אם הגענו לסוף הלוח
            if (row == SudokuConstants.BoardSize)
                return true;

            // תא הבא
            int nextRow = col == SudokuConstants.BoardSize - 1 ? row + 1 : row;
            int nextCol = (col + 1) % SudokuConstants.BoardSize;

            // אם התא מלא, נמשיך לתא הבא
            if (board.GetOptions(row, col).Count == 1)
                return Backtrack(nextRow, nextCol);

            // בדיקה עבור כל ערך אפשרי
            for (int num = SudokuConstants.MinValue; num <= SudokuConstants.MaxValue; num++)
            {
                if (IsValid(row, col, num))
                {
                    // עדכון התא
                    board.GetOptions(row, col).Clear();
                    board.GetOptions(row, col).Add(num);

                    // פתרון רקורסיבי
                    if (Backtrack(nextRow, nextCol))
                        return true;

                    // אם נכשל, ננקה את התא
                    board.GetOptions(row, col).Clear();
                }
            }

            return false;
        }

        private bool IsValid(int row, int col, int num)
        {
            return !GetUsedInRow(row).Contains(num) &&
                   !GetUsedInCol(col).Contains(num) &&
                   !GetUsedInBox(row, col).Contains(num);
        }

        private HashSet<int> GetUsedInRow(int row)
        {
            var used = new HashSet<int>();
            for (int col = 0; col < SudokuConstants.BoardSize; col++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        private HashSet<int> GetUsedInCol(int col)
        {
            var used = new HashSet<int>();
            for (int row = 0; row < SudokuConstants.BoardSize; row++)
            {
                var options = board.GetOptions(row, col);
                if (options.Count == 1)
                    used.Add(options[0]);
            }
            return used;
        }

        private HashSet<int> GetUsedInBox(int row, int col)
        {
            var used = new HashSet<int>();
            int boxRowStart = (row / 3) * 3;
            int boxColStart = (col / 3) * 3;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var options = board.GetOptions(boxRowStart + r, boxColStart + c);
                    if (options.Count == 1)
                        used.Add(options[0]);
                }
            }
            return used;
        }
    }
}
