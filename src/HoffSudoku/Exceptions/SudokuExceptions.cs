using System;

namespace HoffSudoku.Exceptions
{
    // Base class for all custom Sudoku-related exceptions.
    public class SudokuException : Exception
    {
        // Constructor accepting a custom error message.
        public SudokuException(string message) : base(message) { }
    }

    // Represents an error due to invalid input, such as invalid characters or format.
    public class InvalidInputException : SudokuException
    {
        public InvalidInputException(string message) : base(message) { }
    }

    // Represents an error due to an invalid board configuration.
    // Examples: duplicates in rows, columns, or boxes.
    public class InvalidBoardException : SudokuException
    {
        public InvalidBoardException(string message) : base(message) { }
    }

    // Thrown when the board or components are initialized incorrectly.
    public class InitializationException : SudokuException
    {
        public InitializationException(string message) : base(message) { }
    }

    // Thrown when the solver cannot find a solution to the puzzle.
    public class SolveFailureException : SudokuException
    {
        public SolveFailureException(string message) : base(message) { }
    }

    // General runtime exception for unexpected errors during execution.
    public class RuntimeException : SudokuException
    {
        public RuntimeException(string message) : base(message) { }
    }

    // Indicates that the operation was interrupted unexpectedly.
    // Examples: system interrupts like Ctrl+C.
    public class OperationInterruptedException : SudokuException
    {
        public OperationInterruptedException(string message) : base(message) { }
    }
}
