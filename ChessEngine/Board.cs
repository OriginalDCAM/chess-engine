namespace ChessEngine;

public class Board
{
    public HashSet<string> FenList { get; } = [];

    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private ulong[] Bitboard { get; } = new ulong[12];

    public Board()
    {
        GenerateBoardWithFen();
        GenerateBoardWithFen("8/5k2/3p4/1p1Pp2p/pP2Pp1P/P4P1K/8/8 b - - 99 50");
    }

    public void GenerateBoardWithFen(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        Console.WriteLine($"trying to generate board with this fen: {fen}");
        if (Bitboard.Any(bitboard => bitboard != 0)) Array.Clear(Bitboard);

        string[] fenParts = fen.Split(' ');
        
        string[] ranks = fenParts[0].Split('/');
        string startingColor = fenParts[1];
        
        
        if (ranks.Length > 8)
        {
            throw new FormatException("Invalid FEN: " + fen);
        }
        
        PlacePiecesOnBoard(ranks);
        CanMove = startingColor == "w" ? Player.White : Player.Black;

        FenList.Add(fen);
        LastAddedFen = fen;
    }

    private void PlacePiecesOnBoard(string[] ranks)
    {
        for (var rank = 0; rank < 8; rank++)
        {
            var file = 0;
            foreach (char character in ranks[rank])
            {
                if (char.IsDigit(character))
                {
                    file += int.Parse(character.ToString());
                    continue;
                }

                int pieceIndex = Piece.GetPieceIndex(character);
                int squareIndex = rank * 8 + file;
                ulong pieceBit = 1UL << squareIndex;
                Bitboard[pieceIndex] |= pieceBit;
                file++;
            }
        }
    }

    public bool Move(Move move, Player color)
    {
        Console.WriteLine($"{move.StartSquare} to {move.TargetSquare}");
        var moveGen = new MoveGen();
        var board = this;
        List<Move> moves = moveGen.GenerateMoves(ref board, color);
        if (!moves.Contains(move)) return false;
        
        
        ulong fromMask = 1UL << move.StartSquare;
        ulong toMask = 1UL << move.TargetSquare;
        
        int fromBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.StartSquare)); 
        int toBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.TargetSquare));
        

        Bitboard[fromBb] &= ~fromMask; // preform AND operation on the bitboard 
        Bitboard[fromBb] |= toMask; // preforms OR operation on the bitboard
        
        if (toBb != -1) Bitboard[toBb] ^= toMask; // this checks if the piece index is not -1 because that's an invalid index in the bb array. 
        
        CanMove = Player.White == CanMove ? Player.Black : Player.White;
        
        Console.WriteLine($"this player can now move: {CanMove}");
        return true;
    }

    public char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < Bitboard.Length; pieceIndex++)
        {
            ulong bitboard = Bitboard[pieceIndex];
            ulong mask = 1UL << squareIndex;
            if ((bitboard & mask) != 0) return Piece.GetPieceSymbol(pieceIndex);
        }

        return '.';
    }

    public Player GetColorAtSquare(int squareIndex)
    {
        char pieceSymbol = GetPieceSymbolAtSquare(squareIndex);

        return char.IsUpper(pieceSymbol) ? Player.White : Player.Black;
    }
}