namespace ChessEngineTesting;

using ChessEngine;

[TestClass]
public class BoardTest
{
    [TestMethod]
    public void TestPiecePawnPlacement()
    {
        var board = new Board();

        const ulong expectedBlackPawnBb = 0x0000000000ff00;
        const ulong expectedWhitePawnBb = 0xff000000000000;

        Assert.AreEqual(expectedWhitePawnBb, board.PieceBB[(int) Board.Piece.WhitePawn]);
        Assert.AreEqual(expectedBlackPawnBb, board.PieceBB[(int) Board.Piece.BlackPawn]);
    }

    [TestMethod]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    public void TestInitializeBoard()
    {
        // Implement test
        
    }
}