using ChessEngine.Utils;
using Serilog.Parsing;

namespace ChessEngine;

public class Board
{
    public HashSet<string?> FenList { get; } = [];

    public List<MoveHistory> MoveHistory { get; } = new();

    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private ulong[] Bitboards { get; } = new ulong[12];

    public event Action<int, Player> OnPawnPromotion;

    public Board()
    {
        InitializeBoard();
    }


    public void InitializeBoard(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        if (HasNonEmptyBitboard(Bitboards)) Array.Clear(Bitboards);

        string[] fenParts = fen.Split(' ');

        if (fenParts.Length != 6) throw new FormatException("Invalid FEN format:" + fen);

        // Split the ranks, active color, castling availability, en passant target square, half move clock and full move number
        string[] ranks = fenParts[0].Split('/');
        string activeColor = fenParts[1];
        string castlingAvailability = fenParts[2];
        string enPassantTargetSquare = fenParts[3];
        string halfMoveClock = fenParts[4];
        string fullMoveNumber = fenParts[5];

        CastlingRights castlingRights = new CastlingRights
        {
            WhiteKingSide = castlingAvailability.Contains('K'),
            WhiteQueenSide = castlingAvailability.Contains('Q'),
            BlackKingSide = castlingAvailability.Contains('k'),
            BlackQueenSide = castlingAvailability.Contains('q')
        };

        if (ranks.Length > 8) throw new FormatException("Invalid FEN ranks format: " + fen);

        PlacePiecesOnBoard(ranks);
        CanMove = activeColor == "w" ? Player.White : Player.Black;

        FenList.Add(fen);
        LastAddedFen = fen;
    }

    private bool HasNonEmptyBitboard(IEnumerable<ulong> bitboards)
    {
        return bitboards.Any(bitboard => bitboard != 0);
    }

    public IEnumerable<int> GetOccupiedSquares()
    {
        for (var i = 0; i < 64; i++)
        {
            ulong mask = 1UL << i;
            var isOccupied = false;
            for (var pieceIndex = 0;
                 pieceIndex < Enum.GetValues(typeof(Piece.PieceTypes)).Cast<int>().Max();
                 pieceIndex++)
                if ((Bitboards[pieceIndex] & mask) != 0)
                {
                    isOccupied = true;
                    break;
                }

            if (isOccupied) yield return i;
        }
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
                Bitboards[pieceIndex] |= pieceBit;
                file++;
            }
        }
    }

    public bool Move(Move move, Player color)
    {
        Console.WriteLine(
            $"{BoardHelper.ConvertSquareToSan(move.StartSquare)} to {BoardHelper.ConvertSquareToSan(move.TargetSquare)}");
        var moveGen = new MoveGen();
        var moves = moveGen.GenerateMoves( this, color);
        if (!moves.Contains(move)) return false;



        ulong fromMask = 1UL << move.StartSquare;
        ulong toMask = 1UL << move.TargetSquare;

        int fromBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.StartSquare));
        int toBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.TargetSquare));


        Bitboards[fromBb] &= ~fromMask; // preform AND operation on the bitboard 
        Bitboards[fromBb] |= toMask; // preforms OR operation on the bitboard
        
        if (IsPromotion(move.TargetSquare, color) && char.ToLower(GetPieceSymbolAtSquare(move.TargetSquare)) == 'p')
        {
            OnPawnPromotion.Invoke(move.TargetSquare, color);
        }

        if (IsEnPassant(move))
        {
            int direction = color == Player.White ? 8 : -8;
            int capturedPawnSquare = move.TargetSquare + direction;
            ulong capturedPawnMask = 1UL << capturedPawnSquare;
            Console.WriteLine($"Pawn square that needs to be deleted: {capturedPawnSquare}");
            Console.WriteLine(Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare)));
            // Update the opponent's pawn bitboard
            Bitboards[Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare))] &= ~capturedPawnMask;
        }
        else if (toBb != -1)
        {
            Bitboards[toBb] ^= toMask;
        }

        MoveHistory.Add(new MoveHistory(move, color));

        CanMove = Player.White == CanMove ? Player.Black : Player.White;

        Console.WriteLine($"this player can now move: {CanMove}");
        return true;
    }

    private bool IsPromotion(int moveTargetSquare, Player player)
    {
        int rank = moveTargetSquare / 8;
        
        if (player == Player.White && rank == 0) return true;

        if (player == Player.Black && rank == 7) return true;

        return false;
    }

    public char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < Bitboards.Length; pieceIndex++)
        {
            ulong bitboard = Bitboards[pieceIndex];
            ulong mask = 1UL << squareIndex;
            if ((bitboard & mask) != 0) return Piece.GetPieceSymbol(pieceIndex);
        }

        return '.';
    }

    public Player GetColorAtSquare(int squareIndex)
    {
        char pieceSymbol = GetPieceSymbolAtSquare(squareIndex);

        if (pieceSymbol == '.') return Player.Empty;

        return char.IsUpper(pieceSymbol) ? Player.White : Player.Black;
    }

    private bool IsEnPassant(Move move)
    {
        // Check if the moved piece is a pawn
        if (GetPieceSymbolAtSquare(move.TargetSquare) != 'P' && GetPieceSymbolAtSquare(move.TargetSquare) != 'p')
            return false;

        var lastMove = MoveHistory.LastOrDefault();
        // Check for a normal capture instead of en passant
        if (lastMove.Move.TargetSquare / 8 == move.TargetSquare / 8) return false;

        // Check if the pawn moved two squares on its previous move
        if (Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) != 16)
            return false;


        // Check if the current move is a diagonal capture to the square behind the opponent's pawn
        int direction = GetPieceSymbolAtSquare(move.TargetSquare) == 'P' ? -8 : 8;
        return Math.Abs(move.StartSquare - move.TargetSquare) == 9 ||
               (Math.Abs(move.StartSquare - move.TargetSquare) == 7 &&
                GetPieceSymbolAtSquare(move.TargetSquare - direction) != '.');
    }
}