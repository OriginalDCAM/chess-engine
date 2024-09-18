namespace ChessEngine;

public struct CastlingRights
{
    public bool WhiteKingSide { get; set; }
    public bool WhiteQueenSide { get; set; }
    public bool BlackKingSide { get; set; }
    public bool BlackQueenSide { get; set; }
    
    public CastlingRights(bool whiteKingSide, bool whiteQueenSide, bool blackKingSide, bool blackQueenSide)
    {
        WhiteKingSide = whiteKingSide;
        WhiteQueenSide = whiteQueenSide;
        BlackKingSide = blackKingSide;
        BlackQueenSide = blackQueenSide;
    }
    
    public override string ToString()
    {
        return
            $"{(WhiteKingSide ? "K" : "")}{(WhiteQueenSide ? "Q" : "")}{(BlackKingSide ? "k" : "")}{(BlackQueenSide ? "q" : "")}";
    }
}