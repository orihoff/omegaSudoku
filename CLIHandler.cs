using System;

namespace omegaSudoku
{
    public class CLIHandler
    {
        private readonly SudokuBoard board;

        public CLIHandler()
        {
            board = new SudokuBoard();
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Enter puzzle (or 'exit'):");
                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                    break;

                string validationMessage = InputValidator.Validate(input);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    Console.WriteLine($"Invalid input: {validationMessage}");
                    continue;
                }

                try
                {
                    board.Initialize(input);
                    Console.WriteLine("Initial board:");
                    board.Print();

                    SudokuSolver solver = new SudokuSolver(board);
                    bool success = solver.Solve();

                    if (success)
                    {
                        Console.WriteLine("Solved board:");
                        board.Print();
                    }
                    else
                    {
                        Console.WriteLine("Could not fully solve the Sudoku puzzle.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("---------");
            }
        }
    }
}
