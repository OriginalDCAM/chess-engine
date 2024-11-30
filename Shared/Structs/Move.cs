namespace ChessEngine.Structs;

public readonly record struct Move(int StartSquare, int TargetSquare)
{
    public readonly int StartSquare = StartSquare;
    public readonly int TargetSquare = TargetSquare;
}