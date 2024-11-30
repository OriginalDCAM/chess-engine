namespace ChessEngine.Structs;

public readonly record struct SquareInfo(int Square, char Piece, Player Color)
{
    public readonly int Square = Square;
    public readonly char Piece = Piece;
    public readonly Player Color = Color;
}