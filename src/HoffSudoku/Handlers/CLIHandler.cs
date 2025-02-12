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
        private readonly ConsoleView consoleView;

        public CLIHandler()
        {
            fileHandler = new FileHandler();
            consoleView = new ConsoleView();

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
                    string? choice = GetUserChoice();
                    if (choice == "exit")
                    {
                        PrintExitMessage();
                        break;
                    }

                    if (choice != "1" && choice != "2")
                    {
                        ShowErrorMessage("Invalid choice. Please enter '1', '2', or 'exit'.");
                        PauseBeforeContinuing();
                        continue;
                    }

                    string? input = GetPuzzleInput(choice);
                    if (input == null) // If the user chooses to go back to the menu
                    {
                        PauseBeforeContinuing();
                        continue;
                    }

                    ProcessPuzzle(input, choice);
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
        /// Reads the user's menu choice.
        /// </summary>
        private string? GetUserChoice()
        {
            PrintMenuOptions();
            return Console.ReadLine()?.Trim().ToLower();
        }

        /// <summary>
        /// Gets a Sudoku puzzle input from the user via console or file.
        /// </summary>
        private string? GetPuzzleInput(string choice)
        {
            try
            {
                return choice switch
                {
                    "1" => GetPuzzleFromConsole(),
                    "2" => fileHandler.GetPuzzleFromFile(ref originalFilePath),
                    _ => null
                };
            }
            catch (InvalidInputException ex)
            {
                ShowErrorMessage($"Invalid input: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Processes the puzzle input: validates, initializes, solves, and displays/saves the board.
        /// </summary>
        private void ProcessPuzzle(string input, string choice)
        {
            try
            {
                int boardSize = ValidateInputAndGetBoardSize(input);
                SetupBoard(input, boardSize);
                PrintInitialBoard();

                // Validate if the board follows Sudoku rules
                SudokuValidator.ValidateInitialBoard(board);

                SolvePuzzle(choice);
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
            finally
            {
                PauseBeforeContinuing(); // Wait before restarting menu loop
            }
        }

        /// <summary>
        /// Validates input and calculates the board size.
        /// </summary>
        private int ValidateInputAndGetBoardSize(string input)
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
            return size;
        }

        /// <summary>
        /// Initializes the board with the provided input.
        /// </summary>
        private void SetupBoard(string input, int size)
        {
            board = new SudokuBoard();
            board.Initialize(input);
        }

        /// <summary>
        /// Prints the initial board.
        /// </summary>
        private void PrintInitialBoard()
        {
            PrintSectionHeader("Initial Board");
            board.Print();
        }

        /// <summary>
        /// Attempts to solve the puzzle and displays the solved board.
        /// </summary>
        private void SolvePuzzle(string choice)
        {
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
            consoleView.PrintBanner();
        }

        /// <summary>
        /// Displays a welcome message to the user.
        /// </summary>
        private void PrintWelcomeMessage()
        {
            consoleView.PrintWelcomeMessage();
        }

        /// <summary>
        /// Displays the main menu options.
        /// </summary>
        private void PrintMenuOptions()
        {
            consoleView.PrintMenuOptions();
        }

        /// <summary>
        /// Prints a section header with a given title.
        /// </summary>
        private void PrintSectionHeader(string title)
        {
            consoleView.PrintSectionHeader(title);
        }

        /// <summary>
        /// Displays an error message in red.
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            consoleView.ShowErrorMessage(message);
        }

        /// <summary>
        /// Pauses execution until the user presses Enter.
        /// </summary>
        private void PauseBeforeContinuing()
        {
            consoleView.PauseBeforeContinuing();
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
