using System;
using System.IO;
using HoffSudoku.Exceptions;

namespace HoffSudoku.Handlers
{
    public class FileHandler
    {
        public string? GetPuzzleFromFile(ref string? originalFilePath)
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
                    return null;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found. Please ensure the path is correct.");
                    continue;
                }

                try
                {
                    string input = File.ReadAllText(filePath).Replace("\n", "").Replace("\r", "");
                    originalFilePath = filePath;
                    return input;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading the file: {ex.Message}");
                }
            }
        }

        public void SaveSolvedBoardToFile(string solvedBoard, string? originalFilePath)
        {
            while (true)
            {
                Console.WriteLine("Do you want to save the solved board to a new file or overwrite the original file? (Enter 'new', 'overwrite', or 'menu' to return to main menu)");
                string? choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "menu")
                {
                    return;
                }

                string filePath;

                if (choice == "overwrite")
                {
                    if (string.IsNullOrEmpty(originalFilePath))
                    {
                        filePath = PromptForOriginalFilePath();
                        if (filePath == null)
                            return;
                    }
                    else
                    {
                        filePath = originalFilePath;
                    }
                }
                else if (choice == "new")
                {
                    filePath = GetNewFilePath();
                    if (filePath == null)
                        return;
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
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save the file: {ex.Message}");
                }
            }
        }

        private string? PromptForOriginalFilePath()
        {
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
                    return null;
                }

                if (!File.Exists(pathInput))
                {
                    Console.WriteLine("Original file not found for overwriting.");
                    continue;
                }

                return pathInput;
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
                    return null;
                }

                if (File.Exists(newFilePath))
                {
                    Console.WriteLine("File already exists. Do you want to overwrite it? (yes/no or type 'menu' to return to main menu):");
                    string? overwriteChoice = Console.ReadLine()?.Trim().ToLower();

                    if (overwriteChoice == "menu")
                    {
                        return null;
                    }

                    if (overwriteChoice == "yes")
                    {
                        return newFilePath;
                    }
                    else if (overwriteChoice == "no")
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'yes', 'no', or 'menu'.");
                        continue;
                    }
                }

                return newFilePath;
            }
        }
    }
}
