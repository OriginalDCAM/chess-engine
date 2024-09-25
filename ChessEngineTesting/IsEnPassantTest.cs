using ChessEngine;
using ChessEngine.Core;
using ChessEngine.Structs;

namespace ChessEngineTesting;

[TestClass]
public class IsEnPassantTest
{
    private readonly Board _board;

    public IsEnPassantTest()
    {
        _board = new Board();

        _board.InitializeBoard("8/8/8/8/2p4/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

    [TestMethod]
    public void IsEnPassant_Test()
    {
        // Simulate a white pawn move
        _board.Move(new Move(51, 35), Player.White);

        // Create an instance of BoardState with the current board
        var boardState = new BoardState(_board);
    
        // Simulate the black move that should trigger en passant
        var move = new Move(34, 43);

        Assert.IsTrue(boardState.IsEnPassant(move), "Expected IsEnPassant to return true");
    }

    [TestMethod]
    public void IsEnPassant_PawnMovesStraight_ReturnsFalse()
    {
    }

    [TestMethod]
    public void IsEnPassant_NoPreviousMove_ReturnsFalse()
    {
    }

    [TestMethod]
    public void IsEnPassant_OpponentPawnMovedOneSquare_ReturnsFalse()
    {
    }
}