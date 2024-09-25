using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class MoveGen
{
    public List<PieceInfo> FriendlyPieces = [];

    public List<PieceInfo> EnemyPieces = [];

    public Player CanMove;

    private PieceInfo _selectedPiece;

    public List<Move> GenerateMoves(Board board, Player color)
    {
        CanMove = board.CanMove;
        List<Move> moves = [];
        if (color != board.CanMove) return moves;

        foreach (int squareIndex in board.GetOccupiedSquares())
        {
            char pieceSymbol = board.GetPieceSymbolAtSquare(squareIndex);

            if (!char.IsLetter(pieceSymbol)) continue;

            int pieceIndex = Piece.GetPieceIndex(pieceSymbol);

            if (CanMove == color)
            {
                _selectedPiece = new PieceInfo(squareIndex, pieceIndex);
                switch (_selectedPiece.PieceIndex)
                {
                    case (int) Piece.PieceTypes.WhitePawn:
                    case (int) Piece.PieceTypes.BlackPawn:
                        GeneratePawnMoves(ref moves, board);
                        break;
                    case (int) Piece.PieceTypes.WhiteBishop:
                    case (int) Piece.PieceTypes.BlackBishop:
                        GenerateBishopMoves(ref moves, board);
                        break;
                    case (int) Piece.PieceTypes.WhiteRook:
                    case (int) Piece.PieceTypes.BlackRook:
                        GenerateRookMoves(ref moves, board);
                        break;
                    case (int) Piece.PieceTypes.WhiteQueen:
                    case (int) Piece.PieceTypes.BlackQueen:
                        GenerateQueenMoves(ref moves, board);
                        break;
                    case (int) Piece.PieceTypes.WhiteKnight:
                    case (int) Piece.PieceTypes.BlackKnight:
                        GenerateKnightMoves(ref moves, board);
                        break;
                    case (int) Piece.PieceTypes.WhiteKing:
                    case (int) Piece.PieceTypes.BlackKing:
                        // GenerateKingMoves(ref moves, ref board);
                        break;
                }

                FriendlyPieces.Add(new PieceInfo(squareIndex, pieceIndex));
            }
            else
            {
                EnemyPieces.Add(new PieceInfo(squareIndex, pieceIndex));
            }
        }

        return moves;
    }

    private void GenerateKnightMoves(ref List<Move> moves, Board board)
    {
        // List of knight move offsets (L-shaped moves)
        int[] offsets = {10, 6, 15, 17, -10, -6, -17, -15};

        // Get current piece's row and column
        int startSquare = _selectedPiece.SquareIndex;
        int startRank = BoardHelper.GetRankPosition(startSquare);
        int startFile = BoardHelper.GetFilePosition(startSquare);

        foreach (int offset in offsets)
        {
            int targetSquare = startSquare + offset;

            // Check if the move is out of bounds
            if (targetSquare < 0 || targetSquare >= 64) continue;

            // Get target square's row and column
            int targetRank = BoardHelper.GetRankPosition(targetSquare);
            int targetFile = BoardHelper.GetFilePosition(targetSquare);

            // Check if the move stays within knight's L-shaped move bounds (must change row and column)
            int rowDiff = Math.Abs(startRank - targetRank);
            int colDiff = Math.Abs(startFile - targetFile);
            if (!((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2))) continue;

            // Check if the target square is occupied by a friendly piece
            if (board.GetColorAtSquare(targetSquare) == CanMove) continue;
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
        }
    }

    private void GenerateKingMoves(ref List<Move> moves, PieceInfo pieceInfo)
    {
    }

    private void GenerateQueenMoves(ref List<Move> moves, Board board)
    {
        int[] fowardOffsets = [8, -8];
        int[] rankOffsets = [7, 9, -7, -9];
        int[] fileOffsets = [1, -1];

        // Determine the forward pos.
        for (var directionY = 0; directionY < fowardOffsets.Length; directionY++)
        for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 or >= 0;)
        {
            targetSquare += fowardOffsets[directionY];
            if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
            {
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                continue;
            }

            if (board.GetColorAtSquare(targetSquare) == CanMove) break;
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
            break;
        }

        for (var directionY = 0; directionY < rankOffsets.Length; directionY++)
        {
            int filePos = BoardHelper.GetFilePosition(_selectedPiece.SquareIndex);
            int rankPos = BoardHelper.GetRankPosition(_selectedPiece.SquareIndex);
            for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 and >= 0;)
            {
                targetSquare += rankOffsets[directionY];
                int targetFilePos = BoardHelper.GetFilePosition(targetSquare);
                int targetRankPos = BoardHelper.GetRankPosition(targetSquare);

                if (Math.Abs(filePos - targetFilePos) != Math.Abs(rankPos - targetRankPos)) break;

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
        for (var directionX = 0; directionX < fileOffsets.Length; directionX++)
        {
            int filePos = _selectedPiece.SquareIndex % 8;

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

    private void GenerateRookMoves(ref List<Move> moves, Board board)
    {
        int[] verticalOffsets = [8, -8];
        int[] horizontalOffsets = [1, -1];

        for (var i = 0; i < horizontalOffsets.Length; i++)
        {
            int filePos = BoardHelper.GetFilePosition(_selectedPiece.SquareIndex);

            for (int targetPos = filePos + horizontalOffsets[i];
                 targetPos < 8 && targetPos >= 0;
                 targetPos += horizontalOffsets[i])
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

        for (var direction = 0; direction < verticalOffsets.Length; direction++)
        for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 or >= 0;)
        {
            targetSquare += verticalOffsets[direction];
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

    private void GenerateBishopMoves(ref List<Move> moves, Board board)
    {
        int[] offsets = [7, -7, 9, -9];

        for (var direction = 0; direction < offsets.Length; direction++)
        {
            int filePos = BoardHelper.GetFilePosition(_selectedPiece.SquareIndex);
            int rankPos = BoardHelper.GetRankPosition(_selectedPiece.SquareIndex);
            
            for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 and >= 0;)
            {
                targetSquare += offsets[direction];
                int targetFilePos = BoardHelper.GetFilePosition(targetSquare);
                int targetRankPos = BoardHelper.GetRankPosition(targetSquare);

                if (Math.Abs(filePos - targetFilePos) != Math.Abs(rankPos - targetRankPos)) break;

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

    private void GeneratePawnMoves(ref List<Move> moves, Board board)
    {
        int direction = CanMove == Player.White ? -8 : 8;
        int startRank = CanMove == Player.White ? 6 : 1;

        int targetSquare = _selectedPiece.SquareIndex + direction;
        if (board.GetPieceSymbolAtSquare(targetSquare) == '.')
        {
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));

            if (_selectedPiece.SquareIndex / 8 == startRank &&
                board.GetPieceSymbolAtSquare(targetSquare + direction) == '.')
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare + direction));
        }

        int[] offsets = {direction - 1, direction + 1};
        foreach (int offset in offsets)
        {
            targetSquare = _selectedPiece.SquareIndex + offset;
            if (board.GetPieceSymbolAtSquare(targetSquare) != '.' && board.GetColorAtSquare(targetSquare) != CanMove)
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));

            var lastMove = board.MoveHistory.LastOrDefault();

            int previousMoveDifference = lastMove.Move.StartSquare - lastMove.Move.TargetSquare;

            if (previousMoveDifference is 16 or -16)
                if (_selectedPiece.SquareIndex + 1 == lastMove.Move.TargetSquare ||
                    _selectedPiece.SquareIndex - 1 == lastMove.Move.TargetSquare)
                    if (lastMove.Move.TargetSquare + direction == targetSquare)
                        moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
        }
    }
}