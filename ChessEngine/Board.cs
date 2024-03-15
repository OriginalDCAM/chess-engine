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

    public ulong[] PieceBB => _PieceBB;

    private ulong[] _PieceBB = new ulong[12];

    public Board()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        for (var row = 0; row < 8; row++)
        for (var col = 0; col < 8; col++)
        {
            var squareIndex = row * 8 + col;
            var pieceBit = 1UL << squareIndex;

            if (row == 6) _PieceBB[(int) Piece.WhitePawn] |= pieceBit;

            if (row == 1) _PieceBB[(int) Piece.BlackPawn] |= pieceBit;
        }

        _PieceBB[(int) Piece.BlackKing] = 1UL << 4;
        _PieceBB[(int) Piece.BlackQueen] = 1UL << 3;
        _PieceBB[(int) Piece.BlackBishop] = 1UL << 2;
        _PieceBB[(int) Piece.BlackBishop] |= 1UL << 5;
        _PieceBB[(int) Piece.BlackKnight] = 1UL << 1;
        _PieceBB[(int) Piece.BlackKnight] |= 1UL << 6;
        _PieceBB[(int) Piece.BlackRook] = 1UL << 0;
        _PieceBB[(int) Piece.BlackRook] |= 1UL << 7;

        _PieceBB[(int) Piece.WhiteKing] = 1UL << 60;
        _PieceBB[(int) Piece.WhiteQueen] = 1UL << 59;
        _PieceBB[(int) Piece.WhiteBishop] = 1UL << 58;
        _PieceBB[(int) Piece.WhiteBishop] |= 1UL << 61;
        _PieceBB[(int) Piece.WhiteKnight] = 1UL << 57;
        _PieceBB[(int) Piece.WhiteKnight] |= 1UL << 62;
        _PieceBB[(int) Piece.WhiteRook] = 1UL << 56;
        _PieceBB[(int) Piece.WhiteRook] |= 1UL << 63;
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

    private char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < _PieceBB.Length; pieceIndex++)
        {
            var bitboard = _PieceBB[pieceIndex];
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