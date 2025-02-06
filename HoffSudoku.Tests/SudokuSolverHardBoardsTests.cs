using Microsoft.VisualStudio.TestTools.UnitTesting;
using HoffSudoku.Models;
using HoffSudoku.Solvers;
using System;

namespace HoffSudoku.Tests.SolvingAlgorithm
{
    [TestClass]
    public class SudokuSolverHardBoardsTests
    {
        /* Hard board 1. */
        [TestMethod]
        public void HardBoard1Test()
        {
            // Arrange
            string boardString = "800000000003600000070090200050007000000045700000100030001000068008500010090000400";
            string expectedBoardString = "812753649943682175675491283154237896369845721287169534521974368438526917796318452";
            // Set the board size (81 characters = 9x9)
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 2. */
        [TestMethod]
        public void HardBoard2Test()
        {
            // Arrange
            string boardString = "000005080000601043000000000010500000000106000300000005530000061000000004000000000";
            // If the board is unsolvable or remains unchanged, the expected output is identical to input.
            string expectedBoardString = boardString;
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 3. */
        [TestMethod]
        public void HardBoard3Test()
        {
            // Arrange
            string boardString = "030000;062<000@0001;:620@80>700400020@8900035=109>0030000;0=0000=000620080003?05:00008000030000<0@000450;<0000090?4500000900>@0004000<:10>600870000000060008040=0200070050?010<0@873050?0:000200;<060000300000=020>070?0000500:60000501000;020000001006;00098030";
            string expectedBoardString = "73?4=1;562<:9>@85=1;:62<@89>73?4<:62>@89?4735=1;9>@83?471;5=<:62=1;<629:87>@3?45:629@87>453?=1;<>@87?453;<=1:6293?451;<=29:6>@87?45=;<:19>62@8731;<:29>673@8?45=629>873@5=?41;<:@87345=?<:1;629>;<:69>@23?8745=129>@73?8=145;<:6873?5=14:6;<29>@45=1<:6;>@29873?";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 4. */
        [TestMethod]
        public void HardBoard4Test()
        {
            // Arrange
            string boardString = "0000:=000000000?70050;01:00@90<8900800700004600=60:=080000070002=00030890>?500012;01@:000008007>00001000@0000900000<>?0740000000006@900000>0100002;0600=800<00500070002000000000<0900>?5;4020=0@0020=0@0<0907000>?500400=6000<803<0000002001@:0000=68000?0004020";
            string expectedBoardString = ";412:=6@3<8957>?7>?52;41:=6@93<893<8?57>12;46@:=6@:=<893>?57;412=6@:3<897>?52;412;41@:=693<8?57>57>?12;4@:=6893<893<>?57412;=6@::=6@93<857>?12;412;46@:=893<>?57?57>412;6@:=<893<8937>?5;412:=6@412;=6@:<8937>?5>?57;412=6@:3<893<8957>?2;41@:=6@:=6893<?57>412;";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 5. */
        [TestMethod]
        public void HardBoard5Test()
        {
            // Arrange
            string boardString = "000<0000:?00001000000500001<000?00=0?00:00070>0003;0000040957:0004005=0006>?0@00@1>00?00=0000000200=007004@000<:500000;0020000310004200?>0000900>000@00309000200;00010=0000000@>30090080000@070000010;>530600=0800030@?00=0200>40>000000@<04205900@0000490000;00";
            string expectedBoardString = "479<;3@6:?=>85126:8@759>2;1<43=?15=2?84:6@37;>9<?3;>=21<48957:6@:4<75=3816>?9@2;@1>;4?:2=3<95876283=617954@;>?<:59?6<>;@72:8=4317@642:5?>18=<9;3>=1:@4<3;976?285;?2819=7<543:6@>3<59>68;?:2@174=<2419;>5376:@=?89;73:@?18=526<>48>:?376=@<;42159=6@58<249>?13;:7";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 6. */
        [TestMethod]
        public void HardBoard6Test()
        {
            // Arrange
            string boardString = "0700080000063000000070<008?0150006010;0070000?4@000?:10003000000050000;307>0@0040000000500300<97<00000006:10002=02=0970<0@0?:050;00=090000080000005030=0000>0@0?804000:000;300<9>00704@056:00;0060000000><00?000@0040560020;970000098?00100000030;02000000400600";
            string expectedBoardString = "97><@8?4:15632=;2=;37><9@8?4156:56:1=;327><98?4@4@8?:156;32=<97>156:2=;397><@8?4?4@86:15=;32><97<97>4@8?6:15;32=32=;97><4@8?:156;32=<97>?4@86:15:15632=;<97>4@8?8?4@56:12=;37><9><97?4@856:1=;326:15;32=><97?4@8@8?4156:32=;97><7><98?4@156:2=;3=;32><978?4@56:1";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 7. */
        [TestMethod]
        public void HardBoard7Test()
        {
            // Arrange
            string boardString = "1000000<0050:200004?09>00000@70000750004>0198000000000@724000>6100807530:24000>66190000000700:00?:0061008<;00@7050000?020>60000;000980;=00074?:00?02>009=00;03@0000@00?00006;00<00=800000004000>0>00=80;700@240004000000;=0000000700:200000000=80<00300000000000";
            string expectedBoardString = "19>6;=8<@753:24?:24?19>6<;=8@7533@75?:24>6198<;==8<;53@724?:9>61;=8<753@:24?19>6619><;=83@75?:24?:24619>8<;=3@7553@74?:29>61=8<;>6198<;=53@74?:24?:2>619=8<;53@7753@24?:19>6;=8<<;=8@753?:24619>9>61=8<;753@24?:24?:9>61;=8<753@@753:24?619><;=88<;=3@754?:2>619";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 8. */
        [TestMethod]
        public void HardBoard8Test()
        {
            // Arrange
            string boardString = "0E487:009200I300000=<;0?0090:50>00G=1B00;60A<87FE003000=1BC00;0?070008:5@9200D=1<00?080FE450920000006?A0;80FE400092>I0G0010C0E48705I000003G00000100?0<90:0I>B30H10C00F00<;00E4000H>B1000=0000<@E4075I92:CD=060F?A07@E40I00:50B30H00000000000I02:B0GH>10000D=00000A00@94070000I00GH>A00FE00487030:5C0H00600=000009030:00C000?0006000<;2:5I0B000>6?0=1EA<;0@9000G0>B060D01FEA<;94870002:0:5000CD00B?A=004<0F092870H0BC00A010040;02800930:50=1000000000200@0:0I3CDH>000FE40087030:000000C0A=108709230:0IC00>BA=06000<007090:GH5I0D0>B00160A08;F05I00H00>00A006080FE00:000>000=A000048;0E00002G000006?A000;FE2:7@905I3G000000FE402:0@90H003000CDA<100";
            string expectedBoardString = "FE487:5@92H>I3G1BCD=<;6?A@92:5H>I3G=1BCD;6?A<87FE4I3GH>=1BCD<;6?A7FE48:5@92BCD=1<;6?A87FE45@92:H>I3G6?A<;87FE4:5@92>I3GH=1BCDE487@5I92:>B3GH6CD=1;F?A<92:5I>B3GH16CD=F?A<;7@E483GH>B16CD=;F?A<@E4875I92:CD=16;F?A<7@E48I92:5>B3GH?A<;F7@E485I92:B3GH>16CD=D=16?FEA<;@948732:5IBCGH>A<;FE@9487I32:5CGH>B6?D=1487@9I32:5BCGH>?D=16FEA<;2:5I3BCGH>6?D=1EA<;F@9487GH>BC6?D=1FEA<;9487@I32:5:5I3GCDH>B?A=164<;FE9287@H>BCD?A=16E4<;F287@93G:5I=16?AE4<;F9287@G:5I3CDH>B<;FE49287@3G:5IDH>BC?A=1687@923G:5ICDH>BA=16?E4<;F7@92:GH5I3D=>BC<16?A48;FE5I3GHD=>BCA<16?8;FE42:7@9>BCD=A<16?48;FE:7@92GH5I316?A<48;FE2:7@9H5I3GD=>BC;FE482:7@9GH5I3=>BCDA<16?";
            SudokuConstants.SetBoardSize((int)Math.Sqrt(boardString.Length));
            SudokuBoard board = new SudokuBoard();
            board.Initialize(boardString);
            SudokuSolver solver = new SudokuSolver(board);

            // Act
            solver.Solve();

            // Assert
            Assert.AreEqual(expectedBoardString, board.ToString());
        }

        /* Hard board 9. */
        [TestMethod]
        public void HardBoard9Test()
        {
            // Arrange
            string boardString = "213000=<6800000>0:>010000000006000;5079:004200@?<@0080500070420000130@00008;009:000001000<0000065600:0792000?0<@=00?00;5900034210=0@068009:00302042000000068007000900003?=000800;00000074010@?0<:>09342000=<008;000000:00420<0?000000000000:00340000?0<000000:00";
            string expectedBoardString = "2134@?=<68;579:>9:>71342<@?=;56868;5>79:1342=<@?<@?=8;56:>7942134213<@?=568;>79:79:>2134=<@?8;56568;:>792134?=<@=<@?68;59:>73421?=<@568;79:>13423421=<@?;568:>79>79:4213?=<@68;5;5689:>74213@?=<:>793421@?=<568;8;5679:>3421<@?=@?=<;568>79:21341342?=<@8;569:>7";
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
