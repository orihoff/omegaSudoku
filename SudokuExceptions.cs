namespace omegaSudoku.Exceptions
{
    // Base class for all custom Sudoku exceptions
    public class SudokuException : Exception
    {
        public SudokuException(string message) : base(message) { }
    }

    // Exception for invalid input
    public class InvalidInputException : SudokuException
    {
        public InvalidInputException(string message) : base(message) { }
    }

    // Exception for invalid Sudoku board
    public class InvalidBoardException : SudokuException
    {
        public InvalidBoardException(string message) : base(message) { }
    }

    // Exception for initialization errors
    public class InitializationException : SudokuException
    {
        public InitializationException(string message) : base(message) { }
    }

    // Exception for failed puzzle solving
    public class SolveFailureException : SudokuException
    {
        public SolveFailureException(string message) : base(message) { }
    }

    // General runtime exception for unexpected errors
    public class RuntimeException : SudokuException
    {
        public RuntimeException(string message) : base(message) { }
    }
}
