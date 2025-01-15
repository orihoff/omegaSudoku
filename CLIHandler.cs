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

             
                if (input.Length != SudokuConstants.BoardSize * SudokuConstants.BoardSize || !int.TryParse(input, out _))
                {
                    Console.WriteLine($"Invalid input. You entered {input.Length} characters. Please enter a valid {SudokuConstants.BoardSize * SudokuConstants.BoardSize}-character Sudoku puzzle.");
                    continue;
                }

                board.Initialize(input);
                Console.WriteLine("Initial board:");
                board.Print();

                var solver = new SudokuSolver(board);
                if (solver.Solve())
                {
                    Console.WriteLine("Solved board:");
                    board.Print();
                }
                else
                {
                    Console.WriteLine("Could not solve the Sudoku puzzle.");
                }
                Console.WriteLine("---------");
            }
        }
    }
}
