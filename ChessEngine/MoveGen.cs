namespace ChessEngine;

public class MoveGen
{
    public List<PieceInfo> FriendlyPieces;

    public List<PieceInfo> EnemyPieces;

    public Player CanMove;

    private PieceInfo _selectedPiece;

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
            _selectedPiece = new PieceInfo(piece.SquareIndex, piece.PieceIndex); 
            switch (_selectedPiece.PieceIndex)
            {
                case (int) Piece.PieceTypes.WhitePawn:
                case (int) Piece.PieceTypes.BlackPawn:
                    GeneratePawnMoves(ref moves, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteBishop:
                case (int) Piece.PieceTypes.BlackBishop:
                    GenerateBishopMoves(ref moves, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteRook:
                case (int) Piece.PieceTypes.BlackRook:
                    GenerateRookMoves(ref moves, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteQueen:
                case (int) Piece.PieceTypes.BlackQueen:
                    GenerateQueenMoves(ref moves, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteKnight:
                case (int) Piece.PieceTypes.BlackKnight:
                    GenerateKnightMoves(ref moves, ref board);
                    break;
                case (int) Piece.PieceTypes.WhiteKing:
                case (int) Piece.PieceTypes.BlackKing:
                    GenerateKingMoves(ref moves, piece);
                    break;
            }
        }

        return moves;
    }

    private void GenerateKnightMoves(ref List<Move> moves, ref Board board)
    {
        int[] offsets = [10, 6, 15, 17, -10, -6, -17, -15];
        for (int direction = 0; direction < offsets.Length; direction++)
        {
            int targetSquare = _selectedPiece.SquareIndex + offsets[direction];
            if (board.GetColorAtSquare(targetSquare) == CanMove) continue;
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
        }
    }

    private void GenerateKingMoves(ref List<Move> moves, PieceInfo pieceInfo)
    {
    }

    private void GenerateQueenMoves(ref List<Move> moves, ref Board board)
    {
        int[] rankOffsets = [8, -8, 7, 9, -7, -9];
        int[] fileOffsets = [1, -1];

        for (int directionY = 0; directionY < rankOffsets.Length; directionY++)
        {
            for (int targetSquare = _selectedPiece.SquareIndex;;)
            {
                targetSquare += rankOffsets[directionY];
                if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == CanMove) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }

        // Determine the file pos.
        for (int directionX = 0; directionX < fileOffsets.Length; directionX++)
        {
            var filePos = _selectedPiece.SquareIndex % 8;

            for (int targetPos = filePos + fileOffsets[directionX];
                 targetPos < 8 && targetPos >= 0;
                 targetPos += fileOffsets[directionX])
            {
                int difference = targetPos - filePos;
                int targetSquare = _selectedPiece.SquareIndex + difference;

                if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == CanMove) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
    }

    private void GenerateRookMoves(ref List<Move> moves, ref Board board)
    {
        int[] offsets = [8, -8, 1, -1];

        for (int direction = 0; direction < offsets.Length; direction++)
        {
            for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 or >= 0;)
            {
                targetSquare += offsets[direction];
                if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == CanMove) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
    }

    private void GenerateBishopMoves(ref List<Move> moves, ref Board board)
    {
        int[] offsets = [7, -7, 9, -9];

        for (int direction = 0; direction < offsets.Length; direction++)
        {
            for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 and >= 0;)
            {
                targetSquare += offsets[direction];
                
                if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == CanMove) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
    }

    private void GeneratePawnMoves(ref List<Move> moves, ref Board board)
    {
        int direction = CanMove == Player.White ? -8 : 8;
        int startRank = CanMove == Player.White ? 6 : 1;

        int targetSquare = _selectedPiece.SquareIndex + direction;
        if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
        {
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));

            if (_selectedPiece.SquareIndex / 8 == startRank && board.GetPieceSymbolAtSquare(targetSquare + direction) == '.')
            {
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare + direction));
            }
        }

        int[] offsets = {direction - 1, direction + 1};
        foreach (int offset in offsets)
        {
            targetSquare = _selectedPiece.SquareIndex + offset;
            if (board.GetPieceSymbolAtSquare(targetSquare) != '.' && board.GetColorAtSquare(targetSquare) != CanMove)
            {
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
            }

            var lastMove = board.MoveHistory.LastOrDefault();

            var previousMoveDifference = lastMove.Move.StartSquare - lastMove.Move.TargetSquare;

            if (previousMoveDifference is 16 or -16)
            {
                if (_selectedPiece.SquareIndex + 1 == lastMove.Move.TargetSquare ||
                    _selectedPiece.SquareIndex - 1 == lastMove.Move.TargetSquare)
                    if (lastMove.Move.TargetSquare + direction == targetSquare)
                        moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
            }
        }
    }
}