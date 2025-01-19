using System;
using System.Diagnostics;

namespace omegaSudoku
{
    public class SudokuSolver
    {
        private readonly SudokuBoard board;
        private readonly bool[,] rowUsed;
        private readonly bool[,] colUsed;
        private readonly bool[,] boxUsed;

        public SudokuSolver(SudokuBoard board)
        {
            this.board = board;
            int n = SudokuConstants.BoardSize;
            rowUsed = new bool[n, n];
            colUsed = new bool[n, n];
            boxUsed = new bool[n, n];
            InitializeUsed();
        }

        private void InitializeUsed()
        {
            int n = SudokuConstants.BoardSize;
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var opts = board.GetOptions(r, c);
                    if (opts.Count == 1)
                    {
                        int val = opts[0];
                        int idx = (val - SudokuConstants.MinValue) / SudokuConstants.Step;
                        rowUsed[r, idx] = true;
                        colUsed[c, idx] = true;
                        boxUsed[GetBoxIndex(r, c), idx] = true;
                    }
                }
            }
        }

        private int GetBoxIndex(int r, int c)
        {
            return (r / SudokuConstants.SubgridRows) * SudokuConstants.SubgridRows
                 + (c / SudokuConstants.SubgridCols);
        }

        public bool Solve()
        {
            var sw = Stopwatch.StartNew();
            bool solved = Backtrack(0, 0);
            sw.Stop();
            Console.WriteLine($"Time taken to solve: {sw.ElapsedMilliseconds} ms");
            return solved;
        }

        private bool Backtrack(int row, int col)
        {
            int n = SudokuConstants.BoardSize;
            if (row == n) return true;

            int nextCol = (col + 1) % n;
            int nextRow = (nextCol == 0) ? row + 1 : row;

            var opts = board.GetOptions(row, col);
            if (opts.Count == 1)
                return Backtrack(nextRow, nextCol);

            int start = SudokuConstants.MinValue;
            int end = start + (n - 1) * SudokuConstants.Step;

            for (int val = start; val <= end; val += SudokuConstants.Step)
            {
                int idx = (val - start) / SudokuConstants.Step;
                if (rowUsed[row, idx] || colUsed[col, idx] || boxUsed[GetBoxIndex(row, col), idx])
                    continue;

                opts.Clear();
                opts.Add(val);

                rowUsed[row, idx] = true;
                colUsed[col, idx] = true;
                boxUsed[GetBoxIndex(row, col), idx] = true;

                if (Backtrack(nextRow, nextCol))
                    return true;

                board.ResetOptions(row, col);
                rowUsed[row, idx] = false;
                colUsed[col, idx] = false;
                boxUsed[GetBoxIndex(row, col), idx] = false;
            }
            return false;
        }
    }
}
