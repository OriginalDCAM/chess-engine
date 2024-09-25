using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class BoardState
{
    private Board _board;
    
    public BoardState(Board board)
    {
        _board = board;
    }
    
    public bool IsEnPassant(Move move)
    {
        var lastMove = _board.MoveHistory.LastOrDefault();
        if (Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) != 16)
            return false;

        // Ensure the last move was a pawn move and check for the diagonal capture
        int direction = _board.GetPieceSymbolAtSquare(move.TargetSquare) == 'P' ? -8 : 8;
        return Math.Abs(move.StartSquare - move.TargetSquare) == 9 ||
               (Math.Abs(move.StartSquare - move.TargetSquare) == 7 &&
                _board.GetPieceSymbolAtSquare(move.TargetSquare - direction) != '.');
    }
}