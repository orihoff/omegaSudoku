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

            // Set up Ctrl+C (CancelKeyPress) handler
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nCtrl+C detected. Exiting gracefully...");
                e.Cancel = true;
                Environment.Exit(0);
            };

            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public void Run()
        {
            Console.Clear();
            PrintBanner(); 
            PrintWelcomeMessage();

            while (true)
            {
                try
                {
                    PrintMenuOptions(); 

                    string? choice = Console.ReadLine()?.Trim().ToLower();

                    if (choice == "exit")
                        break;

                    if (choice != "1" && choice != "2")
                    {
                        ShowErrorMessage("Invalid choice. Please enter '1', '2', or 'exit'.");
                        PauseBeforeContinuing();
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
                        ShowErrorMessage($"Invalid input: {ex.Message}");
                        PauseBeforeContinuing();
                        continue;
                    }

                    if (input == null)
                    {
                        PauseBeforeContinuing();
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

                        PrintSectionHeader("Initial Board");
                        board.Print();

                        SudokuValidator.ValidateInitialBoard(board);

                        SudokuSolver solver = new SudokuSolver(board);
                        bool success = solver.Solve();

                        if (success)
                        {
                            PrintSectionHeader("Solved Board");
                            board.Print();

                            if (choice == "2")
                            {
                                string solvedBoard = board.ToString();
                                fileHandler.SaveSolvedBoardToFile(solvedBoard, originalFilePath);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\nSolved board has been saved to file.");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            throw new SolveFailureException("Sudoku puzzle is unsolvable.");
                        }
                    }
                    catch (InvalidInputException ex)
                    {
                        ShowErrorMessage($"Invalid input: {ex.Message}");
                    }
                    catch (InvalidBoardException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (SolveFailureException ex)
                    {
                        ShowErrorMessage($"Solver error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Unexpected error: {ex.Message}");
                    }

                    PauseBeforeContinuing();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nOperation interrupted (Ctrl+C). Exiting gracefully...");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Unexpected error: {ex.Message}");
                    PauseBeforeContinuing();
                }
            }
        }

        private string? GetPuzzleFromConsole()
        {
            while (true)
            {
                try
                {
                    PrintSectionHeader("Enter Puzzle");
                    Console.WriteLine("Use '0' for empty cells. Type 'menu' to return to the main menu:");
                    string? input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(input))
                    {
                        ShowErrorMessage("Input cannot be empty.");
                        continue;
                    }

                    if (input.ToLower() == "menu")
                    {
                        return null;
                    }

                    return input;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nOperation interrupted (Ctrl+C). Exiting gracefully...");
                    Environment.Exit(0);
                }
            }
        }

        private void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("               _       _                       _                ");
            Console.WriteLine("  ___  ___   __| |_   _| | ___   _    ___  ___ | |_   _____ _ __ ");
            Console.WriteLine(" / __|/ _ \\ / _` | | | | |/ / | | |  / __|/ _ \\| \\ \\ / / _ \\ '__|");
            Console.WriteLine(" \\__ \\ (_) | (_| | |_| |   <| |_| |  \\__ \\ (_) | |\\ V /  __/ |   ");
            Console.WriteLine(" |___/\\___/ \\__,_|\\__,_|_|\\_\\\\__,_|  |___/\\___/|_| \\_/ \\___|_|   ");
            Console.WriteLine("                                                                 ");
            Console.WriteLine("                  By Ori Hoffnung                    ");
            Console.ResetColor();
            Console.WriteLine("----------------------------------------------------------------");

        }

        private void PrintWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWelcome to the Sudoku solver!");
            Console.WriteLine("You can enter a puzzle manually or load one from a file.");
            Console.ResetColor();
            Console.WriteLine("\n----------------------------------------------------------------");
        }

        private void PrintMenuOptions()
        {
            Console.WriteLine("\n Please choose an option:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  1) Enter a puzzle manually");
            Console.WriteLine("  2) Load a puzzle from file");
            Console.ResetColor();
            Console.WriteLine("  Type 'exit' to quit");
            Console.WriteLine("----------------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        private void PrintSectionHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== {0} ===", title);
            Console.ResetColor();
        }

        private void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void PauseBeforeContinuing()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
