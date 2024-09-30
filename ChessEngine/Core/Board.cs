using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class Board
{
    public HashSet<string?> FenList { get; } = [];

    public List<MoveHistory> MoveHistory { get; } = new();

    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private ulong[] Bitboards { get; } = new ulong[12];

    public event Action<int, Player>? OnPawnPromotion;

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
        Console.WriteLine($"{move.StartSquare} to {move.TargetSquare}");

        var moveGen = new MoveGen();
        var moves = moveGen.GenerateMoves(this, color);
        if (!moves.Contains(move)) return false;

        // Get bitboard indices for the piece at start and target squares
        int startSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.StartSquare));
        int targetSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.TargetSquare));
        
        // Handle pawn promotion
        if (IsPromotion(move.TargetSquare, color) && char.ToLower(GetPieceSymbolAtSquare(move.StartSquare)) == 'p')
        {
            OnPawnPromotion.Invoke(move.TargetSquare, color);
        }

        // Remove the piece from the starting square (clear square)
        BitBoardHelper.ClearSquare(ref Bitboards[startSquarePieceIndex], move.StartSquare);

        // Move the piece to the target square (set square)
        BitBoardHelper.SetSquare(ref Bitboards[startSquarePieceIndex], move.TargetSquare);

        // Handle en passant capture
        var boardState = new BoardState(this);
        if (targetSquarePieceIndex != -1)
        {
            // Handle normal capture
            BitBoardHelper.ClearSquare(ref Bitboards[targetSquarePieceIndex], move.TargetSquare);
        }
        else if (boardState.IsEnPassant(move) && char.ToLower(GetPieceSymbolAtSquare(move.TargetSquare)) == 'p')
        {
            int direction = color == Player.White ? 8 : -8;
            int capturedPawnSquare = move.TargetSquare + direction;

            Console.WriteLine($"Pawn square that needs to be deleted: {capturedPawnSquare}");
            Console.WriteLine($"Piece on square {capturedPawnSquare}: {GetPieceSymbolAtSquare(capturedPawnSquare)}");

            // Remove the captured pawn from the opponent's bitboard
            BitBoardHelper.ClearSquare(ref Bitboards[Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare))],
                capturedPawnSquare);
        }

        // Add move to history
        MoveHistory.Add(new MoveHistory(move, color));

        // Switch turns
        CanMove = Player.White == CanMove ? Player.Black : Player.White;

        Console.WriteLine($"This player can now move: {CanMove}");
        return true;
    }

    private bool IsPromotion(int moveTargetSquare, Player player)
    {
        int rank = moveTargetSquare / 8;

        Console.WriteLine($"Rank: {rank}");

        if (player == Player.White && rank == 0) return true;

        if (player == Player.Black && rank == 7) return true;

        return false;
    }

    public void PromotePawn(int squareIndex, char pieceSymbol)
    {
        char pawnPiece = CanMove == Player.White ? 'p' : 'P';
        Console.WriteLine(
            $"square index: {squareIndex}, pawn piece symbol: {pawnPiece}, promotion piece symbol:{pieceSymbol}");
        BitBoardHelper.ToggleSquare(ref Bitboards[Piece.GetPieceIndex(pawnPiece)], squareIndex);
        BitBoardHelper.SetSquare(ref Bitboards[Piece.GetPieceIndex(pieceSymbol)], squareIndex);
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
}