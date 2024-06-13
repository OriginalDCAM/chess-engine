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
        switch (piece)
        {
            case 'p': return (int) PieceTypes.BlackPawn;
            case 'n': return (int) PieceTypes.BlackKnight;
            case 'b': return (int) PieceTypes.BlackBishop;
            case 'r': return (int) PieceTypes.BlackRook;
            case 'q': return (int) PieceTypes.BlackQueen;
            case 'k': return (int) PieceTypes.BlackKing;
            case 'P': return (int) PieceTypes.WhitePawn;
            case 'N': return (int) PieceTypes.WhiteKnight;
            case 'B': return (int) PieceTypes.WhiteBishop;
            case 'R': return (int) PieceTypes.WhiteRook;
            case 'Q': return (int) PieceTypes.WhiteQueen;
            case 'K': return (int) PieceTypes.WhiteKing;
            default: return -1;
        }
    }

    public static char GetPieceSymbol(int pieceIndex)
    {
        switch (pieceIndex)
        {
            case (int) PieceTypes.WhitePawn: return 'P';
            case (int) PieceTypes.WhiteKnight: return 'N';
            case (int) PieceTypes.WhiteBishop: return 'B';
            case (int) PieceTypes.WhiteRook: return 'R';
            case (int) PieceTypes.WhiteQueen: return 'Q';
            case (int) PieceTypes.WhiteKing: return 'K';
            case (int) PieceTypes.BlackPawn: return 'p';
            case (int) PieceTypes.BlackKnight: return 'n';
            case (int) PieceTypes.BlackBishop: return 'b';
            case (int) PieceTypes.BlackRook: return 'r';
            case (int) PieceTypes.BlackQueen: return 'q';
            case (int) PieceTypes.BlackKing: return 'k';
            default: return ' ';
        }
    }
}