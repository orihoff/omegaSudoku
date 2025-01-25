using System;
using System.IO;
using HoffSudoku;
using omegaSudoku;

namespace HoffSudoku
{
    public class CLIHandler
    {
        private readonly SudokuBoard board;
        private string? originalFilePath; 

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
                Console.WriteLine("Choose input method:\n1. Enter puzzle manually\n2. Load puzzle from file\nType 'exit' to quit.");
                string? choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "exit")
                    break;

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Invalid choice. Please enter '1', '2', or 'exit'.");
                    Console.WriteLine("");
                    continue;
                }

                string? input = null;

                try
                {
                    input = choice switch
                    {
                        "1" => GetPuzzleFromConsole(),
                        "2" => GetPuzzleFromFile(),
                        _ => null // Should not reach here
                    };
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}");
                    Console.WriteLine("");
                    continue;
                }

                if (input == null)
                {
                    // User chose to go back to main menu
                    continue;
                }

                try
                {
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

                        // Handle file saving for file-based input
                        if (choice == "2")
                        {
                            // Get the solved board as text using ToString
                            string solvedBoard = board.ToString(); 
                            SaveSolvedBoardToFile(solvedBoard);
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
                    return null; // Signal to return to main menu
                }

                try
                {
                    // Validate the input format here, if needed
                    InputValidator.Validate(input); 
                    return input;
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"Invalid puzzle input: {ex.Message}");
                    // Loop again to ask for input
                }
            }
        }

        private string? GetPuzzleFromFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path to load the puzzle or type 'menu' to return to main menu:");
                string? filePath = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("File path cannot be empty.");
                    continue;
                }

                if (filePath.ToLower() == "menu")
                {
                    return null; // Signal to return to main menu
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found. Please ensure the path is correct.");
                    continue;
                }

                try
                {
                    string input = File.ReadAllText(filePath);
                    // Validate the input format
                    InputValidator.Validate(input); 
                    originalFilePath = filePath; // Save the file path for possible overwriting
                    return input;
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"Invalid puzzle in file: {ex.Message}");
                    // Loop again to ask for a valid file path or 'menu'
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading the file: {ex.Message}");
                    // Loop again to ask for a valid file path or 'menu'
                }
            }
        }

        private void SaveSolvedBoardToFile(string solvedBoard)
        {
            while (true)
            {
                Console.WriteLine("Do you want to save the solved board to a new file or overwrite the original file? (Enter 'new', 'overwrite', or 'menu' to return to main menu)");
                string? choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "menu")
                {
                    // Return to main menu without saving
                    return;
                }

                string filePath;

                if (choice == "overwrite")
                {
                    if (string.IsNullOrEmpty(originalFilePath))
                    {
                        // If original file path is not available so ask again
                        while (true)
                        {
                            Console.WriteLine("Original file path is not available. Please enter the original file path to overwrite or type 'menu' to return to main menu:");
                            string? pathInput = Console.ReadLine()?.Trim();

                            if (string.IsNullOrEmpty(pathInput))
                            {
                                Console.WriteLine("File path cannot be empty.");
                                continue;
                            }

                            if (pathInput.ToLower() == "menu")
                            {
                                return; // Return to main menu without saving
                            }

                            if (!File.Exists(pathInput))
                            {
                                Console.WriteLine("Original file not found for overwriting.");
                                continue;
                            }

                            filePath = pathInput;
                            break;
                        }
                    }
                    else
                    {
                        // Use the original file path
                        filePath = originalFilePath;
                    }
                }
                else if (choice == "new")
                {
                    filePath = GetNewFilePath();
                    if (filePath == null)
                    {
                        // User chose to return to main menu
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'new', 'overwrite', or 'menu'.");
                    continue;
                }

                try
                {
                    File.WriteAllText(filePath, solvedBoard);
                    Console.WriteLine($"Solved board saved to: {filePath}");
                    break; // Exit the loop after successful save
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save the file: {ex.Message}");
                    // Ask again or allow to go back to main menu
                }
            }
        }

        private string? GetNewFilePath()
        {
            while (true)
            {
                Console.WriteLine("Enter the path for the new file or type 'menu' to return to main menu:");
                string? newFilePath = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(newFilePath))
                {
                    Console.WriteLine("File path cannot be empty.");
                    continue;
                }

                if (newFilePath.ToLower() == "menu")
                {
                    return null; // Signal to return to main menu
                }

                // Check if the file already exists
                if (File.Exists(newFilePath))
                {
                    Console.WriteLine("File already exists. Do you want to overwrite it? (yes/no or type 'menu' to return to main menu):");
                    string? overwriteChoice = Console.ReadLine()?.Trim().ToLower();

                    if (overwriteChoice == "menu")
                    {
                        return null; // Return to main menu without saving
                    }

                    if (overwriteChoice == "yes")
                    {
                        // Proceed to overwrite
                        return newFilePath;
                    }
                    else if (overwriteChoice == "no")
                    {
                        // Ask for a different file path
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'yes', 'no', or 'menu'.");
                        continue;
                    }
                }

                // If file does not exist, proceed to return the path
                return newFilePath;
            }
        }
    }
}
