namespace ChessEngine;

public static class Piece
{
    public enum PieceTypes
    {
        WhitePawn = 0,
        WhiteKnight = 1,
        WhiteBishop = 2,
        WhiteRook = 3,
        WhiteQueen = 4,
        WhiteKing = 5,

        BlackPawn = 6,
        BlackKnight = 7,
        BlackBishop = 8,
        BlackRook = 9,
        BlackQueen = 10,
        BlackKing = 11
    }

    public static int GetPieceIndex(char piece)
    {
        switch (piece)
        {
            case 'P': return (int) PieceTypes.BlackPawn;
            case 'N': return (int) PieceTypes.BlackKnight;
            case 'B': return (int) PieceTypes.BlackBishop;
            case 'R': return (int) PieceTypes.BlackRook;
            case 'Q': return (int) PieceTypes.BlackQueen;
            case 'K': return (int) PieceTypes.BlackKing;
            case 'p': return (int) PieceTypes.WhitePawn;
            case 'n': return (int) PieceTypes.WhiteKnight;
            case 'b': return (int) PieceTypes.WhiteBishop;
            case 'r': return (int) PieceTypes.WhiteRook;
            case 'q': return (int) PieceTypes.WhiteQueen;
            case 'k': return (int) PieceTypes.WhiteKing;
            default: return -1;
        }
    }

    public static char GetPieceSymbol(int pieceIndex)
    {
        switch (pieceIndex)
        {
            case (int) PieceTypes.WhitePawn: return 'p';
            case (int) PieceTypes.WhiteKnight: return 'n';
            case (int) PieceTypes.WhiteBishop: return 'b';
            case (int) PieceTypes.WhiteRook: return 'r';
            case (int) PieceTypes.WhiteQueen: return 'q';
            case (int) PieceTypes.WhiteKing: return 'k';
            case (int) PieceTypes.BlackPawn: return 'P';
            case (int) PieceTypes.BlackKnight: return 'N';
            case (int) PieceTypes.BlackBishop: return 'B';
            case (int) PieceTypes.BlackRook: return 'R';
            case (int) PieceTypes.BlackQueen: return 'Q';
            case (int) PieceTypes.BlackKing: return 'K';
            default: return ' ';
        }
    }
}