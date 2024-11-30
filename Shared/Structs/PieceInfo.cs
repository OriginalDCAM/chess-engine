namespace ChessEngine.Structs;

public struct PieceInfo(int squareIndex, int pieceIndex)
{
    public readonly int SquareIndex = squareIndex;
    public readonly int PieceIndex = pieceIndex;

    public readonly int[] PossibleSquaresAttack;
}