using System;
using HoffSudoku;
using omegaSudoku;

namespace HoffSudoku
{
    public class CLIHandler
    {
        private readonly SudokuBoard board;

        public CLIHandler()
        {
            ValidateAndSetDefaultBoardSize();
            board = new SudokuBoard();
        }

        private void ValidateAndSetDefaultBoardSize()
        {
            try
            {
                // Validate the board size
                SudokuConstants.ValidateBoardSizeAndCalculateSubgridDimensions();
            }
            catch (Exception)
            {
                // If board size is invalid, reset to default (9x9) and notify the user
                Console.WriteLine($"Invalid board size: {SudokuConstants.BoardSize}x{SudokuConstants.BoardSize}. " +
                                  "Board size must have an integer square root. Defaulting to 9x9.");
                SudokuConstants.BoardSize = 9;
                SudokuConstants.ValidateBoardSizeAndCalculateSubgridDimensions();
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Enter puzzle (or 'exit'):");

                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                    break;

                try
                {
                    // Validate the input
                    InputValidator.Validate(input);

                    // Initialize the board
                    board.Initialize(input);

                    Console.WriteLine("Initial board:");
                    board.Print();

                    // Validate the board
                    SudokuValidator.ValidateInitialBoard(board);

                    // Solve the Sudoku
                    SudokuSolver solver = new SudokuSolver(board);
                    bool success = solver.Solve();

                    if (success)
                    {
                        Console.WriteLine("Solved board:");
                        board.Print();
                    }
                    else
                    {
                        throw new SolveFailureException("Could not fully solve the Sudoku puzzle.");
                    }
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}");
                }
                catch (InvalidBoardException ex)
                {
                    // Print only the specific board issue
                    Console.WriteLine(ex.Message);
                }
                catch (SolveFailureException ex)
                {
                    Console.WriteLine($"Solver error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }

                Console.WriteLine("");
            }
        }
    }
}
