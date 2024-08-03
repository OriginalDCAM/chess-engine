namespace ChessEngine;

public static class Piece
{
    public enum PieceTypes
    {
        WhitePawn,
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteQueen,
        WhiteKing,

        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
        BlackKing
    }

    public static int GetPieceIndex(char piece)
    {
        return piece switch
        {
            'p' => (int) PieceTypes.BlackPawn,
            'n' => (int)PieceTypes.BlackKnight,
            'b' => (int)PieceTypes.BlackBishop,
            'r' => (int)PieceTypes.BlackRook,
            'q' => (int)PieceTypes.BlackQueen,
            'k' => (int)PieceTypes.BlackKing,
            'P' => (int)PieceTypes.WhitePawn,
            'N' => (int)PieceTypes.WhiteKnight,
            'B' => (int)PieceTypes.WhiteBishop,
            'R' => (int)PieceTypes.WhiteRook,
            'Q' => (int)PieceTypes.WhiteQueen,
            'K' => (int)PieceTypes.WhiteKing,
            _ => -1
            
        };
    }

    public static char GetPieceSymbol(int pieceIndex)
    {
        return pieceIndex switch
        {
            (int) PieceTypes.WhitePawn => 'P',
            (int) PieceTypes.WhiteKnight => 'N',
            (int) PieceTypes.WhiteBishop => 'B',
            (int) PieceTypes.WhiteRook => 'R',
            (int) PieceTypes.WhiteQueen => 'Q',
            (int) PieceTypes.WhiteKing => 'K',
            (int) PieceTypes.BlackPawn => 'p',
            (int) PieceTypes.BlackKnight => 'n',
            (int) PieceTypes.BlackBishop => 'b',
            (int) PieceTypes.BlackRook => 'r',
            (int) PieceTypes.BlackQueen => 'q',
            (int) PieceTypes.BlackKing => 'k',
            _ => ' '
        };
    }
}