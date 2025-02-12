using System;
using HoffSudoku.Handlers;

namespace HoffSudoku
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                new CLIHandler().Run();
            }
            catch (Exception ex)
            {
                // Handle any unhandled exceptions even they dont exist :)
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
