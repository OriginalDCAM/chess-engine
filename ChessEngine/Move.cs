namespace ChessEngine;

public struct Move(int startSquare, int targetSquare)
{
    public readonly int StartSquare = startSquare;
    public readonly int TargetSquare = targetSquare;
}