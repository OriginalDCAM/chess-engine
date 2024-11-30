using ChessEngine.Structs;

namespace ChessEngine.Utils;

public struct MoveHistory
{
    public Move Move { get; init; }
    public Player Color { get; init; }

    public MoveHistory(Move move, Player color)
    {
        Move = move;
        Color = color;
    }
}