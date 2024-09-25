namespace ChessEngine.Structs;

public record struct Move(int StartSquare, int TargetSquare)
{
    public readonly int StartSquare = StartSquare;
    public readonly int TargetSquare = TargetSquare;
}