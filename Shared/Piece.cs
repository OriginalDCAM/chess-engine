namespace ChessEngine;

public static class Piece
{
    public enum Type
    {
        None = 0,
        Pawn = 1,
        Knight = 2,
        Bishop = 3,
        Rook = 4,
        Queen = 5,
        King = 6
    }

    public enum Color
    {
        White = 0,
        Black = 8
    }

    public enum Pieces
    {
        WhitePawn = Type.Pawn | Color.White,
        WhiteKnight = Type.Knight | Color.White,
        WhiteBishop = Type.Bishop | Color.White,
        WhiteRook = Type.Rook | Color.White,
        WhiteQueen = Type.Queen | Color.White,
        WhiteKing = Type.King | Color.White,

        BlackPawn = Type.Pawn | Color.Black,
        BlackKnight = Type.Knight | Color.Black,
        BlackBishop = Type.Bishop | Color.Black,
        BlackRook = Type.Rook | Color.Black,
        BlackQueen = Type.Queen | Color.Black,
        BlackKing = Type.King | Color.Black
    }

    public static readonly int[] PieceIndices =
    [
        (int) Pieces.WhitePawn,
        (int) Pieces.WhiteKnight,
        (int) Pieces.WhiteBishop,
        (int) Pieces.WhiteRook,
        (int) Pieces.WhiteQueen,
        (int) Pieces.WhiteKing,
        (int) Pieces.BlackPawn,
        (int) Pieces.BlackKnight,
        (int) Pieces.BlackBishop,
        (int) Pieces.BlackRook,
        (int) Pieces.BlackQueen,
        (int) Pieces.BlackKing
    ];
    public static bool IsColor(int piece, int color)
    {
        return (piece & 8) == color;
    }
    
    public static int GetColor(int piece)
    {
        // Check if the color bit is set (Black = 8)
        return (piece & (int) Color.Black) != 0 ? (int) Color.Black : (int) Color.White;
    }
    
    public static bool IsWhite(int piece)
    {
        return IsColor(piece, (int) Color.White);
    } 
    
    public static int GetPieceType(int piece)
    {
        return piece & 7;
    }
    
    public static int GetPieceIndex(char piece)
    {
        piece = char.ToUpper(piece);
        return piece switch
        {
            'P' => (int) Type.Pawn,
            'N' => (int) Type.Knight,
            'B' => (int) Type.Bishop,
            'R' => (int) Type.Rook,
            'Q' => (int) Type.Queen,
            'K' => (int) Type.King,
            _ => (int) Type.None
        };
    }
    
    public static char GetPieceSymbol(int piece)
    {
        int pieceType = GetPieceType(piece);
        
       char symbol = pieceType switch
        {
            (int) Type.Pawn=> 'P',
            (int) Type.Knight => 'N',
            (int) Type.Bishop => 'B',
            (int) Type.Rook => 'R',
            (int) Type.Queen => 'Q',
            (int) Type.King => 'K',

            _ => ' '
        };
        symbol = IsWhite(piece) ? symbol : char.ToLower(symbol);
        
        return symbol;
    }


}