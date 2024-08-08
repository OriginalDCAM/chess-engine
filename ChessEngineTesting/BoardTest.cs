namespace ChessEngineTesting;

using ChessEngine;

[TestClass]
public class BoardTest
{
    [TestMethod]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    public void TestInitializeBoard(string? expectedFen)
    {
        var board = new Board();
        board.GenerateBoardWithFen(expectedFen);

        Assert.AreEqual(board.GetPieceSymbolAtSquare(0), 'r');

        // Implement test
    }
}