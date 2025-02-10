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

            // Handle Ctrl+C (CancelKeyPress) to prevent accidental termination
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nCtrl+C detected. Almost got me, but not this time...");
                PrintExitMessage();
                e.Cancel = true; // Prevents immediate program exit
                Environment.Exit(0);
            };

            // Ensure proper character encoding for display
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// Runs the CLI Sudoku solver, displaying the menu and handling user input.
        /// </summary>
        public void Run()
        {
            Console.Clear();
            PrintBanner(); // Display the Sudoku solver title
            PrintWelcomeMessage(); // Show initial instructions

            while (true)
            {
                try
                {
                    PrintMenuOptions(); // Show the menu options

                    string? choice = Console.ReadLine()?.Trim().ToLower(); // Read user input

                    if (choice == "exit") // Exit condition
                    {
                        PrintExitMessage();
                        break;
                    }

                    if (choice != "1" && choice != "2") // Invalid input handling
                    {
                        ShowErrorMessage("Invalid choice. Please enter '1', '2', or 'exit'.");
                        PauseBeforeContinuing();
                        continue;
                    }

                    string? input = null;
                    try
                    {
                        // Get the puzzle either from console input or a file
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

                    if (input == null) // If the user chooses to go back to the menu
                    {
                        PauseBeforeContinuing();
                        continue;
                    }

                    try
                    {
                        // Verify if input length forms a perfect square (valid Sudoku grid)
                        int inputLength = input.Replace("\n", "").Replace("\r", "").Length;
                        double sqrt = Math.Sqrt(inputLength);
                        if (sqrt % 1 != 0)
                        {
                            throw new InvalidInputException($"Input length {inputLength} is not a perfect square.");
                        }
                        int size = (int)sqrt;

                        // Enforce maximum board size limit (25x25)
                        if (size > 25)
                        {
                            throw new InvalidInputException($"The maximum allowed board size is 25x25. Your board is {size}x{size}.");
                        }

                        // Set board size dynamically
                        SudokuConstants.SetBoardSize(size);

                        // Initialize and display the board
                        board = new SudokuBoard();
                        board.Initialize(input);

                        PrintSectionHeader("Initial Board");
                        board.Print();

                        // Validate if the board follows Sudoku rules
                        SudokuValidator.ValidateInitialBoard(board);

                        // Attempt to solve the Sudoku puzzle
                        SudokuSolver solver = new SudokuSolver(board);
                        bool success = solver.Solve();

                        if (success)
                        {
                            PrintSectionHeader("Solved Board");
                            board.Print();

                            // Save solved puzzle to file if it was loaded from a file
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

                    PauseBeforeContinuing(); // Wait before restarting menu loop
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

        /// <summary>
        /// Gets a Sudoku puzzle input from the user via console.
        /// </summary>
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

                    if (input.ToLower() == "menu") // Allow user to return to main menu
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

        /// <summary>
        /// Prints the ASCII banner with the Sudoku solver title.
        /// </summary>
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

        /// <summary>
        /// Displays a welcome message to the user.
        /// </summary>
        private void PrintWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWelcome to the Sudoku Solver!");
            Console.WriteLine("You can enter a puzzle manually or load one from a file.");
            Console.ResetColor();
            Console.WriteLine("\n----------------------------------------------------------------");
        }

        /// <summary>
        /// Displays the main menu options.
        /// </summary>
        private void PrintMenuOptions()
        {
            Console.WriteLine("\nPlease choose an option:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  1) Enter a puzzle manually");
            Console.WriteLine("  2) Load a puzzle from file");
            Console.ResetColor();
            Console.WriteLine("  Type 'exit' to quit");
            Console.WriteLine("----------------------------------------------------------------");
            Console.Write("Your choice: ");
        }

        /// <summary>
        /// Prints a section header with a given title.
        /// </summary>
        private void PrintSectionHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== {title} ===");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays an error message in red.
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Pauses execution until the user presses Enter.
        /// </summary>
        private void PauseBeforeContinuing()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints the exit message "Goodbye" when exiting the application.
        /// </summary>
        private static void PrintExitMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGoodbye");
            Console.ResetColor();
        }
    }
}
