using HoffSudoku;
using System;
using Xunit;

namespace omegaSudoku.Tests
{
    public class SudokuIntegrationTests
    {
        [Fact]
        public void Solve_ValidPuzzle_ReturnsTrue()
        {
            try
            {
                SudokuConstants.BoardSize = 9;
                SudokuConstants.ValidateBoardSizeAndCalculateSubgridDimensions();

                // Initialize the board
                var board = new SudokuBoard();
                string validPuzzle = "530070000600195000098000060800060003400803001700020006060000280000419005000080079";
                board.Initialize(validPuzzle);

                // Initialize the solver
                var solver = new SudokuSolver(board);

                bool result = solver.Solve();

                // Debugging output
                Console.WriteLine("Solve result: " + result);
                Console.WriteLine("Solved Board:");
                board.Print();

                // Assert
                Assert.True(result, "The solver should solve a valid Sudoku puzzle.");
            }
            catch (Exception ex)
            {
                // Output exception details to help debug
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw; // Rethrow to fail the test
            }
        }
    }
}
