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
                        int idx = val - SudokuConstants.MinValue;
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
            bool solved = Backtrack();
            sw.Stop();
            Console.WriteLine($"Time taken to solve: {sw.ElapsedMilliseconds} ms");
            return solved;
        }

        private bool Backtrack()
        {
            int n = SudokuConstants.BoardSize;
            int minOptionsCount = n + 1;
            int chosenRow = -1;
            int chosenCol = -1;

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var opts = board.GetOptions(r, c);
                    if (opts.Count == 1) continue;

                    int count = 0;
                    foreach (int val in opts)
                    {
                        int idx = val - SudokuConstants.MinValue;
                        if (!rowUsed[r, idx] &&
                            !colUsed[c, idx] &&
                            !boxUsed[GetBoxIndex(r, c), idx])
                        {
                            count++;
                        }
                    }

                    if (count == 0)
                        return false;

                    if (count < minOptionsCount)
                    {
                        minOptionsCount = count;
                        chosenRow = r;
                        chosenCol = c;
                    }
                }
            }

            if (chosenRow == -1 && chosenCol == -1)
                return true;

            var chosenCellOptions = board.GetOptions(chosenRow, chosenCol);

            foreach (int val in chosenCellOptions.ToList())
            {
                int idx = val - SudokuConstants.MinValue;

                if (rowUsed[chosenRow, idx] ||
                    colUsed[chosenCol, idx] ||
                    boxUsed[GetBoxIndex(chosenRow, chosenCol), idx])
                {
                    continue;
                }

                var backupList = new System.Collections.Generic.List<int>(chosenCellOptions);

                chosenCellOptions.Clear();
                chosenCellOptions.Add(val);

                rowUsed[chosenRow, idx] = true;
                colUsed[chosenCol, idx] = true;
                boxUsed[GetBoxIndex(chosenRow, chosenCol), idx] = true;

                if (Backtrack())
                    return true;

                chosenCellOptions.Clear();
                chosenCellOptions.AddRange(backupList);

                rowUsed[chosenRow, idx] = false;
                colUsed[chosenCol, idx] = false;
                boxUsed[GetBoxIndex(chosenRow, chosenCol), idx] = false;
            }

            return false;
        }
    }
}
