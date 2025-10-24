using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

/// <summary>
/// Class <c>Board</c> provides methods to manipulate a chessboard.
/// </summary>
public class Board
{
    private HashSet<string> _fenList = [];

    public HashSet<string> FenList
    {
        get => _fenList;
        set
        {
            _fenList = value;
            LastAddedFen = value.Last();
        }
    }

    public Stack<MoveHistory> MoveHistory { get; } = new();
    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private const int EmptySquareIndex = (int) Player.Empty;
    private const int WhiteColorIndex = (int) Player.White;
    private const int BlackColorIndex = (int) Player.Black;

    private readonly ulong[] _pieceBitboards = new ulong[Piece.maxIndex];
    private readonly ulong[] _colorBitboards = new ulong[2];
    public event Action<int, Player>? OnPawnPromotion;

    public void Init(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        if (BitBoardHelper.HasNonEmptyBitboard(_pieceBitboards)) Array.Clear(_pieceBitboards);

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

        FillBoard(ranks);

        CanMove = activeColor == "w" ? Player.White : Player.Black;

        FenList.Add(fen);
        LastAddedFen = fen;
    }

    public IEnumerable<SquareInfo> GetOccupiedSquares()
    {
        for (var square = 0; square < 64; square++)
        {
            var pieceSymbol = GetPieceSymbolAtSquare(square);
            var color = GetColorAtSquare(square);
            ulong mask = 1UL << square;
            var isOccupied = false;
            for (var pieceIndex = 0;
                 pieceIndex < Piece.maxIndex;
                 pieceIndex++)
                if ((_pieceBitboards[pieceIndex] & mask) != 0)
                {
                    isOccupied = true;
                    break;
                }

            if (isOccupied) yield return new SquareInfo(square, pieceSymbol, color);
        }
    }

    private void FillBoard(string[] ranks)
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

                bool isWhite = char.IsUpper(character);
                int pieceIndex = Piece.GetPieceIndex(character);
                int pieceColor = isWhite ? WhiteColorIndex : BlackColorIndex;
                int squareIndex = rank * 8 + file;
                BitBoardHelper.SetSquare(ref _pieceBitboards[pieceIndex], squareIndex);
                BitBoardHelper.SetSquare(ref _colorBitboards[pieceColor], squareIndex);
                file++;
            }
        }
    }

    public void UnmakeMove()
    {
        bool canPop = MoveHistory.TryPop(out var lastMove);
        Console.WriteLine(MoveHistory.Count);
        if (!canPop) return;
        var move = new Move(lastMove.Move.TargetSquare, lastMove.Move.StartSquare);

        ExecuteMove(move, lastMove.Color);

        if (lastMove.CapturedPiece is not {PieceIndex: 0})
        {
            if (lastMove.CapturedPiece != null)
            {
                int targetPieceIndex = lastMove.CapturedPiece.Value.PieceIndex;
                int targetPieceColor = lastMove.CapturedPiece.Value.PieceColor;

                Console.WriteLine($"{lastMove.CapturedPiece.Value.SquareIndex},{targetPieceColor}, {targetPieceIndex}");

                BitBoardHelper.SetSquare(ref _colorBitboards[targetPieceColor],
                    lastMove.CapturedPiece.Value.SquareIndex);
                BitBoardHelper.SetSquare(
                    ref _pieceBitboards[targetPieceIndex],
                    lastMove.CapturedPiece.Value.SquareIndex);
            }
        }

        CanMove = lastMove.Color;
    }

    public void MakeMove(Move move, Player color)
    {
        if (color != CanMove)
        {
            Console.WriteLine("Not this player's turn");
            return;
        }

        var moveGen = new MoveGen();
        var legalMoves = moveGen.GenerateAllLegalMoves(this, color);

        if (!legalMoves.Contains(move))
        {
            Console.WriteLine("Invalid move attempted.");
            return;
        }

        MoveHistory.Push(ExecuteMove(move, color));

        // Switch turns
        CanMove = CanMove == Player.White ? Player.Black : Player.White;

        Console.WriteLine($"This player can now move: {CanMove}");
    }

    private MoveHistory ExecuteMove(Move move, Player color)
    {
        // Get bitboard indices for the piece at start and target squares
        int startSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.StartSquare));
        int targetSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.TargetSquare));

        var moveData = new MoveHistory
        {
            Move = move,
            Color = color,
            CapturedPiece = null
        };

        // Also get the color index for the current player (0 for white, 1 for black)
        int colorIndex = color == Player.White ? WhiteColorIndex : BlackColorIndex;

        // Handle pawn promotion
        if (BoardHelper.IsPromotion(move.TargetSquare, color) &&
            char.ToLower(GetPieceSymbolAtSquare(move.StartSquare)) == 'p')
        {
            OnPawnPromotion?.Invoke(move.TargetSquare, color);
        }

        // If a piece exists at the target square (i.e., a capture), handle the capture
        if (targetSquarePieceIndex != EmptySquareIndex)
        {
            int targetColorIndex =
                GetColorAtSquare(move.TargetSquare) == Player.White ? WhiteColorIndex : BlackColorIndex;

            moveData.CapturedPiece = new PieceInfo(move.TargetSquare, targetSquarePieceIndex, targetColorIndex);

            // Clear the target piece from its bitboard (piece type)
            BitBoardHelper.ClearSquare(ref _pieceBitboards[targetSquarePieceIndex], move.TargetSquare);
            // Clear the color bitboard for the captured piece's color
            BitBoardHelper.ClearSquare(ref _colorBitboards[targetColorIndex], move.TargetSquare);
        }

        // Handle en passant capture before moving the piece
        var boardState = new BoardState(this);
        bool isMovingPawn = char.ToLower(GetPieceSymbolAtSquare(move.StartSquare)) == 'p';
        if (boardState.IsEnPassant(move) && isMovingPawn)
        {
            int direction = color == Player.White ? 8 : -8;
            int capturedPawnSquare = move.TargetSquare + direction;

            int capturedPawnPieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare));
            int capturedPawnColorIndex =
                GetColorAtSquare(capturedPawnSquare) == Player.White ? WhiteColorIndex : BlackColorIndex;

            moveData.CapturedPiece = new PieceInfo(capturedPawnSquare, capturedPawnPieceIndex, capturedPawnColorIndex);

            BitBoardHelper.ClearSquare(ref _pieceBitboards[capturedPawnPieceIndex], capturedPawnSquare);
            BitBoardHelper.ClearSquare(ref _colorBitboards[capturedPawnColorIndex], capturedPawnSquare);
        }

        // Toggle the start square for the piece's type and move it to the target square
        BitBoardHelper.ToggleSquares(ref _pieceBitboards[startSquarePieceIndex], move.StartSquare, move.TargetSquare);

        // Update the color bitboard: move the color of the piece from start to target square
        BitBoardHelper.ToggleSquares(ref _colorBitboards[colorIndex], move.StartSquare, move.TargetSquare);

        return moveData;
    }

    public void PromotePawn(int squareIndex, char pieceSymbol)
    {
        char pawnPiece = CanMove == Player.White ? 'p' : 'P';
        Console.WriteLine(
            $"square index: {squareIndex}, pawn piece symbol: {pawnPiece}, promotion piece symbol:{pieceSymbol}");
        BitBoardHelper.ToggleSquare(ref _pieceBitboards[Piece.GetPieceIndex(pawnPiece)], squareIndex);
        BitBoardHelper.SetSquare(ref _pieceBitboards[Piece.GetPieceIndex(pieceSymbol)], squareIndex);
    }

    public char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < _pieceBitboards.Length; pieceIndex++)
        {
            ulong bitboard = _pieceBitboards[pieceIndex];
            ulong mask = 1UL << squareIndex;
            if ((bitboard & mask) != 0)
            {
                bool isWhite = (_colorBitboards[0] & mask) != 0;
                int piece = isWhite ? pieceIndex : pieceIndex + 8;
                return Piece.GetPieceSymbol(piece);
            }
        }

        return ' ';
    }

    public Player GetColorAtSquare(int squareIndex)
    {
        char pieceSymbol = GetPieceSymbolAtSquare(squareIndex);

        if (pieceSymbol == ' ') return Player.Empty;

        return char.IsUpper(pieceSymbol) ? Player.White : Player.Black;
    }

    public ulong GetColorBitboard(Player color)
    {
        return _colorBitboards[(int)color];
    }
}