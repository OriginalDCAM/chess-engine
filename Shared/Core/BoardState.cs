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
        var lastMove = Board.MoveHistory.LastOrDefault();
        if (Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) != 16)
            return false;

        // Ensure the last move was a pawn move and check for the diagonal capture
        int direction = Board.GetPieceSymbolAtSquare(move.TargetSquare) == 'P' ? -8 : 8;
        return Math.Abs(move.StartSquare - move.TargetSquare) == 9 ||
               (Math.Abs(move.StartSquare - move.TargetSquare) == 7 &&
                Board.GetPieceSymbolAtSquare(move.TargetSquare - direction) != ' ');
    }
}