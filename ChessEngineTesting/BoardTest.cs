namespace ChessEngineTesting;

using ChessEngine;

[TestClass]
public class BoardTest
{
    [TestMethod]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [DataRow("rn2kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [DataRow("rnbqk3/pppppppp/8/3qk3/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    public void TestInitializeBoard(string expectedFen)
    {
        var board = new Board();
        board.InitializeBoard(expectedFen);

        string[]? fenParts = expectedFen.Split();

        string[] ranks = fenParts[0].Split('/');

        for (var rank = 0; rank < 8; rank++)
        {
            var file = 0;
            foreach (char character in ranks[rank])
            {
                if (char.IsDigit(character))
                {
                    file += int.Parse(character.ToString());
                    continue;
                }

                Assert.AreEqual(board.GetPieceSymbolAtSquare(rank * 8 + file), character);
                file++;
            }
        }
    }
}