using ChessEngine.Structs;

namespace ChessEngine.Utils;

public record struct MoveHistory(Move Move, Player Color, PieceInfo? CapturedPiece = null);