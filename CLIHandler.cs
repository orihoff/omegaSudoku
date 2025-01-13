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

                if (input.Length != 81 || !int.TryParse(input, out _))
                {
                    Console.WriteLine("Invalid input. Please enter a valid 81-character Sudoku puzzle.");
                    continue;
                }

                board.Initialize(input);
                Console.WriteLine("Initial board:");
                board.Print();
                Console.WriteLine("---------");
            }
        }
    }
}
