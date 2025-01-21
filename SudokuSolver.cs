using System;
using System.Diagnostics;
using System.Linq;

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
            int step = SudokuConstants.Step;
            int minValue = SudokuConstants.MinValue;

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var opts = board.GetOptions(r, c);
                    if (opts.Count == 1)
                    {
                        int val = opts[0];
                        if ((val - minValue) % step != 0)
                        {
                            throw new InvalidOperationException($"Value {val} at cell ({r}, {c}) does not conform to the step {step}.");
                        }
                        int idx = (val - minValue) / step;
                        if (idx < 0 || idx >= n)
                        {
                            throw new InvalidOperationException($"Value {val} at cell ({r}, {c}) is out of valid range after step adjustment.");
                        }
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
            int step = SudokuConstants.Step;
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
                        if ((val - SudokuConstants.MinValue) % step != 0)
                            continue;

                        int idx = (val - SudokuConstants.MinValue) / step;
                        if (idx < 0 || idx >= n)
                            continue;

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

            var chosenCellOptions = board.GetOptions(chosenRow, chosenCol).ToList();

            foreach (int val in chosenCellOptions)
            {
                if ((val - SudokuConstants.MinValue) % step != 0)
                    continue;

                int idx = (val - SudokuConstants.MinValue) / step;

                if (idx < 0 || idx >= SudokuConstants.BoardSize)
                    continue;

                if (rowUsed[chosenRow, idx] ||
                    colUsed[chosenCol, idx] ||
                    boxUsed[GetBoxIndex(chosenRow, chosenCol), idx])
                {
                    continue;
                }

                // Backup the current state
                var backupList = new List<int>(chosenCellOptions);

                
                board.SetValue(chosenRow, chosenCol, val);

               
                rowUsed[chosenRow, idx] = true;
                colUsed[chosenCol, idx] = true;
                boxUsed[GetBoxIndex(chosenRow, chosenCol), idx] = true;

                if (Backtrack())
                    return true;

                
                board.SetValue(chosenRow, chosenCol, 0); 
                rowUsed[chosenRow, idx] = false;
                colUsed[chosenCol, idx] = false;
                boxUsed[GetBoxIndex(chosenRow, chosenCol), idx] = false;
            }

            return false;
        }
    }
}
