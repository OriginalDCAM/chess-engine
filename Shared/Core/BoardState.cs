using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class BoardState
{
    public readonly Board Board;
    public BoardState(Board board)
    {
        Board = board;
    }
    
    public bool IsEnPassant(Move move)
    {
        char movingPiece = Board.GetPieceSymbolAtSquare(move.StartSquare);
        if (char.ToLower(movingPiece) != 'p')
            return false;

        if (!Board.MoveHistory.TryPeek(out var lastMove))
            return false;
        
        char lastMovePiece = Board.GetPieceSymbolAtSquare(lastMove.Move.TargetSquare);
        if (char.ToLower(lastMovePiece) != 'p')
            return false;

        bool wasDoublePawnPush = Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) == 16;
        if (!wasDoublePawnPush)
            return false;

        int correctRank = movingPiece == 'P' ? 3 : 4;  // Rank 5 for white, rank 4 for black
        if (move.StartSquare / 8 != correctRank)
            return false;

        int captureSquare = movingPiece == 'P' ? 
            lastMove.Move.TargetSquare - 8 :  // White captures up
            lastMove.Move.TargetSquare + 8;   // Black captures down
        
        Console.WriteLine(lastMove.Move.TargetSquare);

        bool isDiagonalCapture = Math.Abs(move.StartSquare - move.TargetSquare) == 7 || 
                                 Math.Abs(move.StartSquare - move.TargetSquare) == 9;
        
        Console.WriteLine(isDiagonalCapture);
    
        return isDiagonalCapture && move.TargetSquare == captureSquare;
    }
}