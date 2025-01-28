using System;
using HoffSudoku.Models;
using HoffSudoku.Validators;
using HoffSudoku.Solvers;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Handlers
{
    public class CLIHandler
    {
        private SudokuBoard? board;
        private string? originalFilePath;
        private readonly FileHandler fileHandler;

        public CLIHandler()
        {
            fileHandler = new FileHandler();
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Choose input method:\n1. Enter puzzle manually\n2. Load puzzle from file\nType 'exit' to quit.");
                string? choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "exit")
                    break;

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Invalid choice. Please enter '1', '2', or 'exit'.\n");
                    continue;
                }

                string? input = null;

                try
                {
                    input = choice switch
                    {
                        "1" => GetPuzzleFromConsole(),
                        "2" => fileHandler.GetPuzzleFromFile(ref originalFilePath),
                        _ => null
                    };
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}\n");
                    continue;
                }

                if (input == null)
                {
                    continue;
                }

                try
                {
                    int inputLength = input.Replace("\n", "").Replace("\r", "").Length;
                    double sqrt = Math.Sqrt(inputLength);
                    if (sqrt % 1 != 0)
                    {
                        throw new InvalidInputException($"Input length {inputLength} is not a perfect square.");
                    }
                    int size = (int)sqrt;

                    SudokuConstants.SetBoardSize(size);

                    board = new SudokuBoard();
                    board.Initialize(input);

                    Console.WriteLine("Initial board:");
                    board.Print();

                    SudokuValidator.ValidateInitialBoard(board);

                    SudokuSolver solver = new SudokuSolver(board);
                    bool success = solver.Solve();

                    if (success)
                    {
                        Console.WriteLine("Solved board:");
                        board.Print();

                        if (choice == "2")
                        {
                            string solvedBoard = board.ToString();
                            fileHandler.SaveSolvedBoardToFile(solvedBoard, originalFilePath);
                        }
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

        private string? GetPuzzleFromConsole()
        {
            while (true)
            {
                Console.WriteLine("Enter puzzle (use 0 for empty cells) or type 'menu' to return to main menu:");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty.");
                    continue;
                }

                if (input.ToLower() == "menu")
                {
                    return null;
                }

                return input;
            }
        }
    }
}
