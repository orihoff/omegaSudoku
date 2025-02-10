using Microsoft.VisualStudio.TestTools.UnitTesting;
using HoffSudoku.Models;
using HoffSudoku.Solvers;
using System;
// Note: There is always the option to validate the solution dynamically 
// using SudokuValidator.ValidateInitialBoard(board) instead of explicitly 
// specifying the expected output. However, I have decided to keep the current 
// approach of comparing the solved board to a predefined expected result. 
// This maintains consistency with previous projects like the calculator(when we inserted the expected result) and ensures a direct 
// verification of the solver's correctness. Additionally, using dynamic validation 
// does not verify whether the originally fixed cells in the board remain unchanged, 
// which is an important aspect of ensuring the solver preserves the given constraints.

namespace HoffSudoku.Tests.SolvingAlgorithm
{
    [TestClass]
    public class SudokuSolverEasyBoardsTests
    {
        /* Easy board 1. */
        [TestMethod]
        public void EasyBoard1Test()
        {
            // Arrange
            string boardString = "003000002080050000700800049000000100006003000900500078009060014000400200100000500";
            string expectedBoardString = "693741852482659731715832649257984163846173925931526478579268314368415297124397586";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 2. */
        [TestMethod]
        public void EasyBoard2Test()
        {
            // Arrange
            string boardString = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        string expectedBoardString = "146287359528639147793514268275346981931758624684921573452893716317462895869175432";

        
        SudokuConstants.SetBoardSize((int) Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
        board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

        // Act
        solver.Solve();

            
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

    /* Easy board 3. */
    [TestMethod]
        public void EasyBoard3Test()
        {
            // Arrange
            string boardString = "800000070006010053040600000000080400003000700020005038000000800004050061900002000";
            string expectedBoardString = "831529674796814253542637189159783426483296715627145938365471892274958361918362547";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 4. */
        [TestMethod]
        public void EasyBoard4Test()
        {
            // Arrange
            string boardString = "8001?796000:000007000:4000000@=1<002500;=0000000000;8@0190?000000008746000:00005000?030<;00=@0000020000010@9040000;5@9086070002<0;0000000@004000005>000@07420000968@0207000;=050000030<:0>0006002<000003000000@000:000>=0000000000>06?00040<;0006?@00000:3;0180=";
            string expectedBoardString = "8@=1?79642<:5>3;?796<:423;5>8@=1<:425>3;=18@?7965>3;8@=196?7<:42@918746?2<:3>=;5746?:32<;5>=@918:32<>=;518@9746?>=;5@9186?74:32<3;<:=15>8@9642?7=15>968@?7423;<:968@42?7<:3;=15>42?73;<:5>=1968@2<74;5:3>=186?@9;5:318>=@96?2<7418>=6?@9742<;5:36?@92<74:3;518>=";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 5. */
        [TestMethod]
        public void EasyBoard5Test()
        {
            // Arrange
            string boardString = ">000?006;00=20000@000090700:0300300;080000096?0060000=;3000000<90>40000000300000820004<007600005;300@0020>40760:700:05=080009000076?00500820<0000900000:0=03080000209>40000000;30;002@00<0>0:700@002<0000?0000=0500000@040900:00000600350182000>0090:700300;0180";
            string expectedBoardString = ">4<9?:76;35=2@182@184<9>76?:;35=35=;182@>4<96?:76?:75=;32@18>4<99>4<6?:7=;3582@182@1>4<9:76?=;35;35=@1829>4<76?:76?:35=;82@19>4<:76?;35=182@<9>4<9>476?:5=;3182@182@9>4<?:765=;3=;352@18<9>4:76?@182<9>46?:735=;5=;382@14<9>?:76?:76=;35@1824<9>4<9>:76?35=;@182";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 6. */
        [TestMethod]
        public void EasyBoard6Test()
        {
            // Arrange
            string boardString = "509600000030807920000300800000016080050000010000000032104030000006709000000000003";
            string expectedBoardString = "589624371431857926267391845743216589652983714918475632194538267326749158875162493";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 7. */
        [TestMethod]
        public void EasyBoard7Test()
        {
            // Arrange
            string boardString = "600040010010000003002008040020000004007382600500000020090500100400000070050090002";
            string expectedBoardString = "635249817814756293972138546326915784147382659589467321293574168461823975758691432";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 8. */
        [TestMethod]
        public void EasyBoard8Test()
        {
            // Arrange
            string boardString = "100042700200003008057001000000000060090000100000715200000000002018009003900054000";
            string expectedBoardString = "189642735264573918357891426571928364892436157643715289435187692718269543926354871";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 9. */
        [TestMethod]
        public void EasyBoard9Test()
        {
            // Arrange
            string boardString = "003080600820000000006075080000000750900050806017006000000007010000500000000064937";
            string expectedBoardString = "753182649821649375496375182268493751934751826517826493642937518379518264185264937";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 10. */
        [TestMethod]
        public void EasyBoard10Test()
        {
            // Arrange
            string boardString = "040287000000000000309000002507034600400000008200870009000020090060700400000041006";
            string expectedBoardString = "641287935728593164359416872587934621493162758216875349134628597862759413975341286";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 11.
           1x1
         */
        [TestMethod]
        public void EasyBoard11Test()
        {
            // Arrange
            string boardString = "0";
            string expectedBoardString = "1";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Easy board 12.
           4x4
        */
        [TestMethod]
        public void EasyBoard12Test()
        {
            // Arrange
            string boardString = "0000000000000000";
            string expectedBoardString = "1324421324313142";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* super Easy board 13. 1x1 */
        [TestMethod]
        public void EasyBoard13Test()
        {
            // Arrange
            string boardString = "0";
            string expectedBoardString = "1";

            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

    }
}
