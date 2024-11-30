using System.Collections;
using ChessEngine.Extensions;
using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core;

public class MoveGen
{
    public readonly List<PieceInfo> FriendlyPieces = [];

    public readonly List<PieceInfo> EnemyPieces = [];

    private Player _color;

    private PieceInfo _selectedPiece;

    public IList<Move> GenerateAllLegalMoves(Board board, Player color)
    {
        IList<Move> moves = [];
        _color = color;

        Console.WriteLine(board.BoardState);
        
        if (board.BoardState == GameStates.Checkmate)
        {
            Console.WriteLine("No moves available, in check");
            return moves;
        }

        

        foreach (SquareInfo info in board.GetOccupiedSquares())
        {
            char pieceSymbol = board.GetPieceSymbolAtSquare(info.Square);
            if (!char.IsLetter(pieceSymbol)) continue;

            int pieceIndex = Piece.GetPieceIndex(pieceSymbol);

            if (color != board.GetColorAtSquare(info.Square))
            {
                EnemyPieces.Add(new PieceInfo(info.Square, pieceIndex));
                continue;
            }

            // Generate moves for current player's pieces
            _selectedPiece = new PieceInfo(info.Square, pieceIndex);
            switch (_selectedPiece.PieceIndex)
            {
                case (int) Piece.Type.Pawn:
                    GeneratePawnMoves(ref moves, board);
                    break;
                case (int) Piece.Type.Bishop:
                    GenerateBishopMoves(ref moves, board);
                    break;
                case (int) Piece.Type.Rook:
                    GenerateRookMoves(ref moves, board);
                    break;
                case (int) Piece.Type.Queen:
                    GenerateQueenMoves(ref moves, board);
                    break;
                case (int) Piece.Type.Knight:
                    GenerateKnightMoves(ref moves, board);
                    break;
                case (int) Piece.Type.King:
                    GenerateKingMoves(ref moves, board);
                    break;
            }

            FriendlyPieces.Add(_selectedPiece);
        }

        return moves;
    }


    private void GenerateKnightMoves(ref IList<Move> moves, Board board)
    {
        // List of knight move offsets (L-shaped moves)
        int[] offsets = [10, 6, 15, 17, -10, -6, -17, -15];

        // Get current piece's row and column
        int startSquare = _selectedPiece.SquareIndex;
        int startRank = BoardHelper.GetRankPosition(startSquare);
        int startFile = BoardHelper.GetFilePosition(startSquare);

        foreach (int offset in offsets)
        {
            int targetSquare = startSquare + offset;

            // Check if the move is out of bounds
            if (targetSquare is < 0 or >= 64) continue;

            // Get target square's row and column
            int targetRank = BoardHelper.GetRankPosition(targetSquare);
            int targetFile = BoardHelper.GetFilePosition(targetSquare);

            // Check if the move stays within knight's L-shaped move bounds (must change row and column)
            int rowDiff = Math.Abs(startRank - targetRank);
            int colDiff = Math.Abs(startFile - targetFile);
            if (!((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2))) continue;

            // Check if the target square is occupied by a friendly piece
            if (board.GetColorAtSquare(targetSquare) == _color) continue;
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
        }
    }

    private void GenerateKingMoves(ref IList<Move> moves, Board board)
    {
        int[] offsets = [8, -8, 1, -1, 7, 9, -7, -9];

        foreach (int offset in offsets)
        {
            int targetSquare = _selectedPiece.SquareIndex + offset;

            // Check if the move is out of bounds
            if (targetSquare is < 0 or >= 64) continue;

            // Check if the target square is occupied by a friendly piece
            if (board.GetColorAtSquare(targetSquare) == _color) continue;

            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
        }
    }

    private void GenerateQueenMoves(ref IList<Move> moves, Board board)
    {
        int[] rankOffsets = [8, -8];
        int[] diagonalOffsets = [7, 9, -7, -9];
        int[] fileOffsets = [1, -1];

        // Determine the forward pos.
        foreach (int rankOffset in rankOffsets)
            for (int targetSquare = _selectedPiece.SquareIndex;;)
            {
                targetSquare += rankOffset;
                
                if (targetSquare is < 0 or >= 64) break;
                
                if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == _color) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }

        foreach (int diagonalOffset in diagonalOffsets)
        {
            int filePos = BoardHelper.GetFilePosition(_selectedPiece.SquareIndex);
            int rankPos = BoardHelper.GetRankPosition(_selectedPiece.SquareIndex);
            for (int targetSquare = _selectedPiece.SquareIndex; targetSquare is < 64 and >= 0;)
            {
                targetSquare += diagonalOffset;
                
                if (targetSquare is < 0 or >= 64) break;
                int targetFilePos = BoardHelper.GetFilePosition(targetSquare);
                int targetRankPos = BoardHelper.GetRankPosition(targetSquare);

                if (Math.Abs(filePos - targetFilePos) != Math.Abs(rankPos - targetRankPos)) break;

                if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == _color) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }

        // Determine the file pos.
        foreach (int fileOffset in fileOffsets)
        {
            int filePos = _selectedPiece.SquareIndex % 8;

            for (int targetPos = filePos + fileOffset;
                 targetPos < 8 && targetPos >= 0;
                 targetPos += fileOffset)
            {
                int difference = targetPos - filePos;
                int targetSquare = _selectedPiece.SquareIndex + difference;
                
                if (targetSquare is < 0 or >= 64) continue;

                if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == _color) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
    }

    private void GenerateRookMoves(ref IList<Move> moves, Board board)
    {
        int[] rankOffsets = [8, -8];
        int[] fileOffsets = [1, -1];

        foreach (int fileOffset in fileOffsets)
        {
            int filePos = BoardHelper.GetFilePosition(_selectedPiece.SquareIndex);

            for (int targetPos = filePos + fileOffset;
                 targetPos < 8 && targetPos >= 0;
                 targetPos += fileOffset)
            {
                int difference = targetPos - filePos;
                int targetSquare = _selectedPiece.SquareIndex + difference;

                if (targetSquare is < 0 or >= 64) break;

                if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == _color) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
        foreach (int rankOffset in rankOffsets)
        {
            int targetSquare = _selectedPiece.SquareIndex;
            targetSquare += rankOffset;
            
            if (targetSquare is < 0 or >= 64) continue;
            
            if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
            {
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                continue;
            }

            if (board.GetColorAtSquare(targetSquare) == _color) break;
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
            break;
        }
    }

    private void GenerateBishopMoves(ref IList<Move> moves, Board board)
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

                if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
                {
                    moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                    continue;
                }

                if (board.GetColorAtSquare(targetSquare) == _color) break;
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
                break;
            }
        }
    }

    private void GeneratePawnMoves(ref IList<Move> moves, Board board)
    {
        int direction = _color == Player.White ? -8 : 8;
        int startRank = _color == Player.White ? 6 : 1;

        int targetSquare = _selectedPiece.SquareIndex + direction;
        if (board.GetPieceSymbolAtSquare(targetSquare) == ' ')
        {
            moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));

            if (_selectedPiece.SquareIndex / 8 == startRank &&
                board.GetPieceSymbolAtSquare(targetSquare + direction) == ' ')
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare + direction));
        }

        int[] offsets = [direction - 1, direction + 1]; // Check for left and right diagonal
        foreach (int offset in offsets)
        {
            targetSquare = _selectedPiece.SquareIndex + offset;

            // Regular diagonal capture check
            if (board.GetPieceSymbolAtSquare(targetSquare) != ' ' && board.GetColorAtSquare(targetSquare) != _color)
            {
                moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare));
            }

            var lastMove = board.MoveHistory.LastOrDefault();

            // Check if the last move was a pawn moving two squares forward
            if (Math.Abs(lastMove.Move.StartSquare - lastMove.Move.TargetSquare) == 16)
            {
                // Check if the last move's target is directly adjacent to the current pawn
                if (_selectedPiece.SquareIndex + 1 == lastMove.Move.TargetSquare ||
                    _selectedPiece.SquareIndex - 1 == lastMove.Move.TargetSquare)
                {
                    // Ensure the current pawn is capturing diagonally to an empty square
                    if (board.GetPieceSymbolAtSquare(targetSquare) == ' ' &&
                        lastMove.Move.TargetSquare + direction == targetSquare)
                    {
                        moves.Add(new Move(_selectedPiece.SquareIndex, targetSquare)); // Add en passant move
                    }
                }
            }
        }
    }
}