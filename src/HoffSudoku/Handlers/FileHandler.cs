using System;
using System.IO;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Handlers
{
    public class FileHandler
    {
        /// <summary>
        /// Prompts the user for a file path and loads a Sudoku puzzle from the specified file.
        /// If the user types 'menu', the function returns to the main menu.
        /// </summary>
        public string? GetPuzzleFromFile(ref string? originalFilePath)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter the file path to load the puzzle or type 'menu' to return to the main menu:");
                    string? filePath = Console.ReadLine()?.Trim();

                    // Ensure the file path is not empty.
                    if (string.IsNullOrEmpty(filePath))
                    {
                        Console.WriteLine("File path cannot be empty.");
                        continue;
                    }

                    // Return to the menu if requested.
                    if (filePath.ToLower() == "menu")
                    {
                        return null;
                    }

                    // Validate the file existence.
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File not found. Please ensure the path is correct.");
                        continue;
                    }

                    // Read the file content and return it.
                    string input = File.ReadAllText(filePath);
                    originalFilePath = filePath;
                    return input;
                }
                // Handle specific exceptions with meaningful messages.
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Access to the file is denied. Please check file permissions.");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("The specified file was not found. Please verify the file path.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An error occurred while accessing the file: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Saves the solved Sudoku board back to the original file.
        /// If the original file path is unavailable, an error message is displayed.
        /// </summary>
        public void SaveSolvedBoardToFile(string solvedBoard, string? originalFilePath)
        {
            // Ensure the original file path is available before attempting to save.
            if (string.IsNullOrEmpty(originalFilePath))
            {
                Console.WriteLine("Error: Original file path is not available. Cannot save the solved board.");
                return;
            }

            try
            {
                // Write the solved board to the original file.
                File.WriteAllText(originalFilePath, solvedBoard);
                Console.WriteLine($"Solved board saved to: {originalFilePath}");
            }
            // Handle possible file write errors.
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access to the file is denied. Please check file permissions.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
