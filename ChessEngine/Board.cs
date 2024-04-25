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

    public List<string> FenList { get; private set; } = new();

    public ulong[] Bitboard { get; } = new ulong[12];

    public Board()
    {
        GenerateBoardWithFen("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2");
        PrintChessboard();
    }

    public void GenerateBoardWithFen(string fen)
    {
        Console.WriteLine("Generating board with FEN: " + fen);
        if (Bitboard.Any(bitboard => bitboard != 0)) Array.Clear(Bitboard);
        
        string[] fenParts = fen.Split(' ');
        string[] ranks = fenParts[0].Split('/');

        try
        {
            if (ranks.Length != 8) throw new ArgumentException("Invalid FEN: " + fen);

            for (var rank = 0; rank < 8; rank++)
            {
                var file = 0;
                foreach (var character in ranks[rank])
                {
                    if (char.IsDigit(character))
                    {
                        file += int.Parse(character.ToString());
                        continue;
                    }

                    var pieceIndex = GetPieceIndex(character);
                    var squareIndex = rank * 8 + file;
                    var pieceBit = 1UL << squareIndex;
                    Bitboard[pieceIndex] |= pieceBit;
                    file++;
                }
            }

            FenList.Add(fen);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private int GetPieceIndex(char piece)
    {
        switch (piece)
        {
            case 'p': return (int) Piece.BlackPawn;
            case 'n': return (int) Piece.BlackKnight;
            case 'b': return (int) Piece.BlackBishop;
            case 'r': return (int) Piece.BlackRook;
            case 'q': return (int) Piece.BlackQueen;
            case 'k': return (int) Piece.BlackKing;
            case 'P': return (int) Piece.WhitePawn;
            case 'N': return (int) Piece.WhiteKnight;
            case 'B': return (int) Piece.WhiteBishop;
            case 'R': return (int) Piece.WhiteRook;
            case 'Q': return (int) Piece.WhiteQueen;
            case 'K': return (int) Piece.WhiteKing;
            default: return -1;
        }
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