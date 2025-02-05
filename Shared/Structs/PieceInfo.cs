namespace ChessEngine.Structs;

public struct PieceInfo(int squareIndex, int pieceIndex, int pieceColor)
{
    public readonly int SquareIndex = squareIndex;
    public readonly int PieceIndex = pieceIndex;
    public readonly int PieceColor = pieceColor;

    public readonly int[] PossibleSquaresAttack;
}