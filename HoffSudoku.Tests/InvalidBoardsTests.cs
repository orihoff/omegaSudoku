using Microsoft.VisualStudio.TestTools.UnitTesting;
using HoffSudoku.Models;
using HoffSudoku.Exceptions;
using System;
using HoffSudoku.Validators;

namespace HoffSudoku.Tests.Validation
{
    [TestClass]
    public class InvalidBoardsTests
    {
        [TestMethod]
        public void InvalidCharacterTest_CheckExceptionMessage()
        {
            // Arrange: create a 9x9 board string with an invalid character 'X' inserted in the 5th row.
            string boardString =
                "000000000" +
                "000000000" +
                "000000000" +
                "000000000" +
                "00000000X" + // Invalid character 'X'
                "000000000" +
                "000000000" +
                "000000000" +
                "000000000";

            // Set board size (9x9) based on the length of the string.
            int boardSize = (int)Math.Sqrt(boardString.Length);
            HoffSudoku.Models.SudokuConstants.SetBoardSize(boardSize);
            var board = new HoffSudoku.Models.SudokuBoard();

            // Act & Assert: The call to Initialize should throw an InvalidInputException with the expected message.
            var ex = Assert.ThrowsException<InvalidInputException>(() => board.Initialize(boardString));
            string expectedMessage = "Input contains an invalid character: 'X'. Expected characters between '1' and '9', or '0' for an empty cell.";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestMethod]
        public void NonPerfectSquareBoardTest()
        {
            // Arrange: input board string of length 5.
            string boardString = "00000";
            // Expected error message is now updated to match your implementation.
            string expectedMessage = "Invalid board size: 5x5. Board size must have an integer square root.";

            // Act & Assert:
            // Instead of computing the board size as (int)Math.Sqrt(boardString.Length),
            
            var ex = Assert.ThrowsException<InvalidInputException>(() =>
                SudokuConstants.SetBoardSize(boardString.Length)
            );
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestMethod]
        public void DuplicateInBoxTest()
        {
            // Arrange
            // This board string has 81 characters (9x9).
            // It is arranged such that one of the 3x3 boxes contains duplicate digit '5'.
            string boardString = "500005080000601043000000000010500000000106000300000005530000061000000004000000000";

            // Set board size for 9x9.
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);

            // Act & Assert:
            // ValidateInitialBoard should throw an InvalidBoardException because of the duplicate in one box.
            Assert.ThrowsException<InvalidBoardException>(() =>
                SudokuValidator.ValidateInitialBoard(board)
            );
        }


    }
}
