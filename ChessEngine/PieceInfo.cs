namespace ChessEngine;

public struct PieceInfo
{
    public readonly int SquareIndex;
    public readonly int PieceIndex;

    public PieceInfo(int squareIndex, int pieceIndex)
    {
        SquareIndex = squareIndex;
        PieceIndex = pieceIndex;
    }
}