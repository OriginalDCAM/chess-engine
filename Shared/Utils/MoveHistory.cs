using ChessEngine.Structs;

namespace ChessEngine.Utils;

public record struct MoveHistory(Move Move, Player Color, int PieceIndex, PieceInfo? CapturedPiece = null);