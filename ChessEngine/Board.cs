namespace ChessEngine;

public class Board
{
    public enum Piece
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

    public ulong[] Bitboard { get; } = new ulong[12];

    public Board()
    {
        GenerateBoard();
        PrintChessboard();
    }

    public void GenerateBoard()
    {
        for (var row = 0; row < 8; row++)
        for (var col = 0; col < 8; col++)
        {
            var squareIndex = row * 8 + col;
            var pieceBit = 1UL << squareIndex;

            if (row == 6) Bitboard[(int) Piece.WhitePawn] |= pieceBit;

            if (row == 1) Bitboard[(int) Piece.BlackPawn] |= pieceBit;
        }

        Bitboard[(int) Piece.BlackKing] = 1UL << 4;
        Bitboard[(int) Piece.BlackQueen] = 1UL << 3;
        Bitboard[(int) Piece.BlackBishop] = 1UL << 2;
        Bitboard[(int) Piece.BlackBishop] |= 1UL << 5;
        Bitboard[(int) Piece.BlackKnight] = 1UL << 9;
        Bitboard[(int) Piece.BlackKnight] |= 1UL << 6;
        Bitboard[(int) Piece.BlackRook] = 1UL << 0;
        Bitboard[(int) Piece.BlackRook] |= 1UL << 7;

        Bitboard[(int) Piece.WhiteKing] = 1UL << 60;
        Bitboard[(int) Piece.WhiteQueen] = 1UL << 59;
        Bitboard[(int) Piece.WhiteBishop] = 1UL << 58;
        Bitboard[(int) Piece.WhiteBishop] |= 1UL << 61;
        Bitboard[(int) Piece.WhiteKnight] = 1UL << 57;
        Bitboard[(int) Piece.WhiteKnight] |= 1UL << 62;
        Bitboard[(int) Piece.WhiteRook] = 1UL << 56;
        Bitboard[(int) Piece.WhiteRook] |= 1UL << 63;
    }

    private string GetPieceSymbol(int pieceIndex)
    {
        switch (pieceIndex)
        {
            case (int) Piece.WhitePawn: return "p";
            case (int) Piece.WhiteKnight: return "n";
            case (int) Piece.WhiteBishop: return "b";
            case (int) Piece.WhiteRook: return "r";
            case (int) Piece.WhiteQueen: return "q";
            case (int) Piece.WhiteKing: return "k";
            case (int) Piece.BlackPawn: return "P";
            case (int) Piece.BlackKnight: return "N";
            case (int) Piece.BlackBishop: return "B";
            case (int) Piece.BlackRook: return "R";
            case (int) Piece.BlackQueen: return "Q";
            case (int) Piece.BlackKing: return "K";
            default: return " ";
        }
    }

    public char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < Bitboard.Length; pieceIndex++)
        {
            var bitboard = Bitboard[pieceIndex];
            var mask = 1UL << squareIndex;
            if ((bitboard & mask) != 0) return GetPieceSymbol(pieceIndex)[0];
        }

        return '.';
    }

    public void PrintChessboard()
    {
        for (var rank = 0; rank < 8; rank++)
        {
            for (var fileIndex = 0; fileIndex < 8; fileIndex++)
            {
                var squareIndex = rank * 8 + fileIndex;
                var pieceSymbol = GetPieceSymbolAtSquare(squareIndex);
                Console.Write(pieceSymbol + " ");
            }

            Console.WriteLine();
        }
    }
}