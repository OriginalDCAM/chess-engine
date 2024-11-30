using System.Diagnostics;
using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class Board
{
    private HashSet<string?> _fenList = [];

    public HashSet<string?> FenList
    {
        get => _fenList;
        set
        {
            _fenList = value;
            LastAddedFen = value.Last();
        }
    }

    public List<MoveHistory> MoveHistory { get; set; } = new();

    public GameStates BoardState { get; set; }

    public Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; private set; }

    private const int _whiteColorIndex = (int) Player.White;
    private const int _blackColorIndex = (int) Player.Black;

    private ulong[]? _pieceBitboards;
    private ulong[]? _colorBitboards;
    public event Action<int, Player>? OnPawnPromotion;

    public void Init(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        BoardState = GameStates.Normal;
        _pieceBitboards = new ulong[typeof(Piece.Pieces).GetEnumValues().Cast<int>().Max()];
        _colorBitboards = new ulong[2];
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
        int maxPieceIndex = Enum.GetValues(typeof(Piece.Pieces)).Cast<int>().Max();

        for (var square = 0; square < 64; square++)
        {
            var pieceSymbol = GetPieceSymbolAtSquare(square);
            var color = GetColorAtSquare(square);
            ulong mask = 1UL << square;
            var isOccupied = false;
            for (var pieceIndex = 0;
                 pieceIndex < maxPieceIndex;
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
                int pieceColor = isWhite ? 0 : 1;
                Console.WriteLine("Piece index: " + pieceIndex);
                int squareIndex = rank * 8 + file;
                BitBoardHelper.SetSquare(ref _pieceBitboards[pieceIndex], squareIndex);
                BitBoardHelper.SetSquare(ref _colorBitboards[pieceColor], squareIndex);
                file++;
            }
        }
    }

    public void MakeMove(Move move, Player color)
    {
        if (move.StartSquare < 0 || move.StartSquare > 63 || move.TargetSquare < 0 || move.TargetSquare > 63)
        {
            Console.WriteLine("Invalid move");
            return;
        }

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

        // Execute the move
        ExecuteMove(move, color);

        // Check if the king is still in check after the move
        if (IsInCheck(color))
        {
            BoardState = GameStates.Checkmate;
            // Revert to the original state if the king is in check
            return;
        }

        // Player opponentColor = CanMove == Player.White ? Player.Black : Player.White;
        // if (IsCheckmate(opponentColor))
        // {
            // BoardState = GameStates.Checkmate;
        // }

        // Add the move to history
        MoveHistory.Add(new MoveHistory(move, color));

        // Switch turns
        CanMove = CanMove == Player.White ? Player.Black : Player.White;

        Console.WriteLine($"This player can now move: {CanMove}");
    }

    public void UnMakeMove(Move move, Player color)
    {
        if (MoveHistory.Count == 0)
        {
            Console.WriteLine("No moves to undo");
            return;
        }
        
        // Get the last move from the history
        var lastMove = MoveHistory.Last().Move;
        
        if (lastMove != move)
        {
            Console.WriteLine("Invalid move to undo");
            return;
        }
        
    }

    public void ExecuteMove(Move move, Player color)
    {
        // Get bitboard indices for the piece at start and target squares
        int startSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.StartSquare));
        int targetSquarePieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(move.TargetSquare));

        // Also get the color index for the current player (0 for white, 1 for black)
        int colorIndex = color == Player.White ? _whiteColorIndex : _blackColorIndex;

        // Handle pawn promotion
        if (BoardHelper.IsPromotion(move.TargetSquare, color) &&
            char.ToLower(GetPieceSymbolAtSquare(move.StartSquare)) == 'p')
        {
            OnPawnPromotion?.Invoke(move.TargetSquare, color);
        }

        // If a piece exists at the target square (i.e., a capture), handle the capture
        if (targetSquarePieceIndex != -1)
        {
            // Clear the target piece from its bitboard (piece type)
            BitBoardHelper.ClearSquare(ref _pieceBitboards[targetSquarePieceIndex], move.TargetSquare);

            // Get the color of the captured piece (before clearing)
            Player targetSquareColor = GetColorAtSquare(move.TargetSquare);

            // Clear the color bitboard for the captured piece's color
            int targetColorIndex = targetSquareColor == Player.White ? _blackColorIndex : _whiteColorIndex;
            BitBoardHelper.ClearSquare(ref _colorBitboards[targetColorIndex], move.TargetSquare);
        }


        // Toggle the start square for the piece's type and move it to the target square
        BitBoardHelper.ToggleSquares(ref _pieceBitboards[startSquarePieceIndex], move.StartSquare, move.TargetSquare);

        // Update the color bitboard: move the color of the piece from start to target square
        BitBoardHelper.ToggleSquares(ref _colorBitboards[colorIndex], move.StartSquare, move.TargetSquare);

        // Handle en passant capture
        var boardState = new BoardState(this);
        if (boardState.IsEnPassant(move) && char.ToLower(GetPieceSymbolAtSquare(move.TargetSquare)) == 'p')
        {
            int direction = color == Player.White ? 8 : -8;
            int capturedPawnSquare = move.TargetSquare + direction;

            Console.WriteLine($"Pawn square that needs to be deleted: {capturedPawnSquare}");
            Console.WriteLine($"Piece on square {capturedPawnSquare}: {GetPieceSymbolAtSquare(capturedPawnSquare)}");

            // Remove the captured pawn from the opponent's bitboard
            int capturedPawnPieceIndex = Piece.GetPieceIndex(GetPieceSymbolAtSquare(capturedPawnSquare));
            BitBoardHelper.ClearSquare(ref _pieceBitboards[capturedPawnPieceIndex], capturedPawnSquare);

            // Also clear the color bitboard for the captured pawn
            int capturedPawnColorIndex = GetColorAtSquare(capturedPawnSquare) == Player.White ? 0 : 1;
            BitBoardHelper.ClearSquare(ref _colorBitboards[capturedPawnColorIndex], capturedPawnSquare);
        }
    }

    public bool IsCheckmate(Player kingColor)
    {
        // First, check if the king is in check. If not, it's not checkmate.
        if (!IsInCheck(kingColor)) return false;

        // Initialize move generator
        var moveGen = new MoveGen();

        // Generate all legal moves for the kingColor player
        var allLegalMoves = moveGen.GenerateAllLegalMoves(this, kingColor);

        // Simulate each move to see if it gets the king out of check
        foreach (var move in allLegalMoves)
        {
            // Create a deep copy of the board
            Board boardCopy = Clone();
            boardCopy.MakeMove(move, kingColor);

            // If the move results in the king no longer being in check, it's not checkmate
            if (!boardCopy.IsInCheck(kingColor))
            {
                return false; // King can escape, not checkmate
            }
            
            // If the move results in the king still being in check, add the attack square to the list
        }
        return true;
    }


    public bool IsInCheck(Player kingColor)
    {
        // Get the king's position
        char kingSymbol = kingColor == Player.White ? 'K' : 'k';
        int kingSquare = GetOccupiedSquares()
            .First(square => GetPieceSymbolAtSquare(square.Square) == kingSymbol)
            .Square;

        // Get opponent's color
        Player opponentColor = kingColor == Player.White ? Player.Black : Player.White;

        // Check if any of the opponent's pieces can attack the king
        var moveGen = new MoveGen();

        // Generate all moves for the opponent's pieces
        var allLegalMoves = moveGen.GenerateAllLegalMoves(this, opponentColor);

        // Check if any of the opponent's moves target the king's square
        foreach (var move in allLegalMoves)
        {
            if (move.TargetSquare == kingSquare)
            {
                return true; // King is in check
            }
        }

        return false; // King is not in check
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

    public Board Clone()
    {
        // Step 1: Create a new instance of the Board
        Board clone = new Board();

        clone.CanMove = CanMove;
        clone.LastAddedFen = LastAddedFen;
        clone._pieceBitboards = (ulong[]) _pieceBitboards.Clone();
        clone._colorBitboards = (ulong[]) _colorBitboards.Clone();
        clone.MoveHistory = new List<MoveHistory>(MoveHistory.Count);
        foreach (var move in MoveHistory)
        {
            clone.MoveHistory.Add(move); // Assuming MoveHistoryItem has a Clone() method
        }

        // Step 4: Return the clone
        return clone;
    }
}