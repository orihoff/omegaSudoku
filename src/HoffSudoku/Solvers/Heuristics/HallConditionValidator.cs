using System;
using System.Collections.Generic;
using System.Numerics;
using HoffSudoku.Models;
using HoffSudoku.Helpers;

namespace HoffSudoku.Solvers
{
    /// /// <summary>
    /// Validates the Hall condition for a 9x9 board.
    /// Hall's Condition is a mathematical rule that ensures a set of numbers 
    /// can fit into a limited number of available spots. 
    /// In Sudoku, this means that if a group of missing numbers in a row, column, 
    /// or box has fewer available cells than numbers, then the puzzle is unsolvable. 
    /// This condition helps to detect impossible situations early, 
    /// preventing wasted effort in solving an unsolvable puzzle.
    /// </summary>

    public class HallConditionValidator
    {
        public static bool ValidateHallCondition(SudokuBoard board, int[] rowMask, int[] colMask, int[] boxMask)
        {
            // Only run this check for standard 9x9 boards.
            if (SudokuConstants.BoardSize != 9)
                return true;

            int n = SudokuConstants.BoardSize;
            int fullMask = (1 << n) - 1; // Mask with all digits set.

            // Validate each row.
            for (int r = 0; r < n; r++)
            {
                int fixedMask = 0;
                List<int> cellCandidates = new List<int>();
                for (int c = 0; c < n; c++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1)
                    {
                        fixedMask |= options;
                    }
                    else
                    {
                        int cand = options & ~(rowMask[r] | colMask[c] | boxMask[SudokuHelper.GetBoxIndex(r, c)]);
                        if (cand == 0)
                            return false; // No candidates -> contradiction.
                        cellCandidates.Add(cand);
                    }
                }
                int missing = fullMask & ~fixedMask; // Missing digits in the row.
                if (!ValidateHallForUnit(cellCandidates, missing))
                    return false;
            }

            // Validate each column.
            for (int c = 0; c < n; c++)
            {
                int fixedMask = 0;
                List<int> cellCandidates = new List<int>();
                for (int r = 0; r < n; r++)
                {
                    int options = board.GetOptions(r, c);
                    if (SudokuHelper.CountBits(options) == 1)
                    {
                        fixedMask |= options;
                    }
                    else
                    {
                        int cand = options & ~(rowMask[r] | colMask[c] | boxMask[SudokuHelper.GetBoxIndex(r, c)]);
                        if (cand == 0)
                            return false;
                        cellCandidates.Add(cand);
                    }
                }
                int missing = fullMask & ~fixedMask;
                if (!ValidateHallForUnit(cellCandidates, missing))
                    return false;
            }

            // Validate each subgrid.
            int subRows = SudokuConstants.SubgridRows;
            int subCols = SudokuConstants.SubgridCols;
            for (int br = 0; br < n; br += subRows)
            {
                for (int bc = 0; bc < n; bc += subCols)
                {
                    int fixedMask = 0;
                    List<int> cellCandidates = new List<int>();
                    for (int r = br; r < br + subRows; r++)
                    {
                        for (int c = bc; c < bc + subCols; c++)
                        {
                            int options = board.GetOptions(r, c);
                            if (SudokuHelper.CountBits(options) == 1)
                            {
                                fixedMask |= options;
                            }
                            else
                            {
                                int cand = options & ~(rowMask[r] | colMask[c] | boxMask[SudokuHelper.GetBoxIndex(r, c)]);
                                if (cand == 0)
                                    return false;
                                cellCandidates.Add(cand);
                            }
                        }
                    }
                    int missing = fullMask & ~fixedMask;
                    if (!ValidateHallForUnit(cellCandidates, missing))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates the Hall condition for a unit (row, column, or subgrid).
        /// </summary>
        private static bool ValidateHallForUnit(List<int> cellCandidates, int missing)
        {
            if (missing == 0)
                return true; // All digits are present.

            // Check every subset of missing digits.
            for (int s = missing; s > 0; s = (s - 1) & missing)
            {
                int requiredCount = SudokuHelper.CountBits(s); // Number of digits in the subset.
                int availableCells = 0;
                foreach (int cand in cellCandidates)
                {
                    if ((cand & s) != 0)
                        availableCells++; // Cell can host a digit from the subset.
                }
                if (availableCells < requiredCount)
                    return false; // Not enough cells for these digits.
            }
            return true;
        }
    }
}
