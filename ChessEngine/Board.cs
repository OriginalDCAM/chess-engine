using ChessEngine.Utils;
using Serilog.Parsing;

namespace ChessEngine;

public class Board
{
    public HashSet<string> FenList { get; } = [];

    public List<MoveHistory> MoveHistory { get; init; } = new List<MoveHistory>();

    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private ulong[] Bitboard { get; } = new ulong[12];

    public Board()
    {
        GenerateBoardWithFen();
    }

    public void GenerateBoardWithFen(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        Console.WriteLine($"trying to generate board with this fen: {fen}");
        if (Bitboard.Any(bitboard => bitboard != 0)) Array.Clear(Bitboard);

        string[] fenParts = fen.Split(' ');

        string[] ranks = fenParts[0].Split('/');
        string startingColor = fenParts[1];
        Console.WriteLine(startingColor);


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

        if (IsEnPassant(move, board))
        {
            var direction = color == Player.White ? 8 : -8;
            int capturedPawnSquare = move.TargetSquare + direction;
            ulong capturedPawnMask = 1UL << capturedPawnSquare;
            Console.WriteLine($"Pawn square that needs to be deleted: {capturedPawnSquare}");
            Console.WriteLine(Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare)));
            // Update the opponent's pawn bitboard
            Bitboard[Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare))] &= ~capturedPawnMask;
        }
        else if (toBb != -1)
        {
            Bitboard[toBb] ^= toMask;
        }

        MoveHistory.Add(new MoveHistory(move, color));

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

    private bool IsEnPassant(Move move, Board board)
    {
        // Check if the moved piece is a pawn
        Console.WriteLine(GetPieceSymbolAtSquare(move.TargetSquare));
        if (GetPieceSymbolAtSquare(move.TargetSquare) != 'P' && GetPieceSymbolAtSquare(move.TargetSquare) != 'p')
            return false;
        // Check if the pawn moved two squares on its previous move
        var lastMove = board.MoveHistory.LastOrDefault();
        if (Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) != 16)
            return false;

        // Check if the current move is a diagonal capture to the square behind the opponent's pawn
        int direction = GetPieceSymbolAtSquare(move.TargetSquare) == 'P' ? -8 : 8;
        Console.WriteLine(Math.Abs(move.StartSquare - move.TargetSquare));
        Console.WriteLine(board.GetPieceSymbolAtSquare(move.TargetSquare - direction));
        return Math.Abs(move.StartSquare - move.TargetSquare) == 9 ||
               Math.Abs(move.StartSquare - move.TargetSquare) == 7 &&
               board.GetPieceSymbolAtSquare(move.TargetSquare - direction) != '.';
    }
}