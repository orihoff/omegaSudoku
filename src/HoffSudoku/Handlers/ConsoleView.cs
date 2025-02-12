using System;

namespace HoffSudoku.Handlers
{
    public class ConsoleView
    {
        /// <summary>
        /// Prints the ASCII banner.
        /// </summary>
        public void PrintBanner()
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
        /// Prints the welcome message.
        /// </summary>
        public void PrintWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWelcome to the Sudoku Solver!");
            Console.WriteLine("You can enter a puzzle manually or load one from a file.");
            Console.ResetColor();
            Console.WriteLine("\n----------------------------------------------------------------");
        }

        /// <summary>
        /// Prints the menu options.
        /// </summary>
        public void PrintMenuOptions()
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
        /// Prints a section header with the specified title.
        /// </summary>
        public void PrintSectionHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== {title} ===");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays an error message in red.
        /// </summary>
        public void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Pauses execution until the user presses Enter.
        /// </summary>
        public void PauseBeforeContinuing()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints the exit message.
        /// </summary>
        public void PrintExitMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGoodbye");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays a general message in the specified color.
        /// </summary>
        public void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
