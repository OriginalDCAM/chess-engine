namespace ChessEngine;

public class MoveGen
{
    public List<PieceInfo> FriendlyPieces;

    public List<PieceInfo> EnemyPieces;

    public Player CanMove;

    public List<Move> GenerateMoves(ref Board board, Player color)
    {
        CanMove = board.CanMove;
        List<Move> moves = [];
        if (color != board.CanMove) return moves;

        FriendlyPieces = [];
        EnemyPieces = [];
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            var pieceSymbol = board.GetPieceSymbolAtSquare(squareIndex);

            if (!char.IsLetter(pieceSymbol)) continue;

            var pieceIndex = Piece.GetPieceIndex(pieceSymbol);

            if ((char.IsLower(pieceSymbol) && board.CanMove == Player.Black) ||
                (char.IsUpper(pieceSymbol) && board.CanMove == Player.White))
            {
                FriendlyPieces.Add(new PieceInfo(squareIndex, pieceIndex));
            }
            else
            {
                EnemyPieces.Add(new PieceInfo(squareIndex, pieceIndex));
            }
        }

        foreach (var piece in FriendlyPieces)
        {
            switch (piece.PieceIndex)
            {
                case (int) Piece.PieceTypes.WhitePawn:
                case (int) Piece.PieceTypes.BlackPawn:
                    GeneratePawnMoves(ref moves, piece, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteBishop:
                case (int) Piece.PieceTypes.BlackBishop:
                    GenerateBishopMoves(ref moves, piece, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteRook:
                case (int) Piece.PieceTypes.BlackRook:
                    GenerateRookMoves(ref moves);
                    break;
                case (int) Piece.PieceTypes.WhiteQueen:
                case (int) Piece.PieceTypes.BlackQueen:
                    GenerateQueenMoves(ref moves);
                    break;
                case (int) Piece.PieceTypes.WhiteKing:
                case (int) Piece.PieceTypes.BlackKing:
                    GenerateKingMoves(ref moves, piece);
                    break;
            }
        }


        return moves;
    }

    private void GenerateKingMoves(ref List<Move> moves, PieceInfo pieceInfo)
    {
    }

    private void GenerateQueenMoves(ref List<Move> moves)
    {
    }

    private void GenerateRookMoves(ref List<Move> moves)
    {
    }

    private void GenerateBishopMoves(ref List<Move> moves, PieceInfo pieceInfo, ref Board board)
    {
        var currentSquare = pieceInfo.SquareIndex;

        int[] offset = [7,-7,9,-9];
        
        for (int direction = 0; direction < offset.Length; direction++)
        {
            for (int targetSquare = currentSquare; targetSquare is < 64 and >= 0;)
            {
                targetSquare += offset[direction];
                moves.Add(new Move(currentSquare, targetSquare));
                if (board.GetPieceSymbolAtSquare(targetSquare) != '.') break;
                
                
            }
        }

    }

    private void GeneratePawnMoves(ref List<Move> moves, PieceInfo pieceInfo, ref Board board)
    {
        int direction = CanMove == Player.White ? -8 : 8;
        int startRank = CanMove == Player.White ? 6 : 1;

        int targetSquare = pieceInfo.SquareIndex + direction;
        if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
        {
            moves.Add(new Move(pieceInfo.SquareIndex, targetSquare));

            if (pieceInfo.SquareIndex / 8 == startRank && board.GetPieceSymbolAtSquare(targetSquare + direction) == '.')
            {
                moves.Add(new Move(pieceInfo.SquareIndex, targetSquare + direction));
            }
        }

        int[] offsets = {direction - 1, direction + 1};
        foreach (int offset in offsets)
        {
            targetSquare = pieceInfo.SquareIndex + offset;
            if (board.GetPieceSymbolAtSquare(targetSquare) != '.' && board.GetColorAtSquare(targetSquare) != CanMove)
            {
                moves.Add(new Move(pieceInfo.SquareIndex, targetSquare));
            }

            var lastMove = board.MoveHistory.LastOrDefault();

            var previousMoveDifference = lastMove.Move.StartSquare - lastMove.Move.TargetSquare;

            if (previousMoveDifference is 16 or -16)
            {
                if (pieceInfo.SquareIndex + 1 == lastMove.Move.TargetSquare ||
                    pieceInfo.SquareIndex - 1 == lastMove.Move.TargetSquare)
                    if (lastMove.Move.TargetSquare + direction == targetSquare)
                        moves.Add(new Move(pieceInfo.SquareIndex, targetSquare));
            }
        }
    }
}