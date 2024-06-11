using ChessEngine;
using Microsoft.AspNetCore.Components;

namespace ChessWebUI.Components;

public partial class Chessboard
{
    [Parameter] public required Board Board { get; set; }
    
    private int _selectedSquare = -1; // Track selected square

    private void OnSquareClick(int square)
    {
        if (_selectedSquare == -1) // No piece selected, select this one
        {
            if (Board.GetPieceSymbolAtSquare(square) != '.')
            {
                _selectedSquare = square;
            }
        }
        else // Piece already selected, move it
        {
            Move(_selectedSquare, square, Board.GetColorAtSquare(_selectedSquare));
            _selectedSquare = -1; // Deselect after move
        }
    }

    private void Move(int from, int to, Player colour)
    {
        foreach (var bitboard in Board.Bitboard)
        {
            var hasMoved = Board.Move(from, to, bitboard, colour);
            if (hasMoved) break;
        }
    }

    private void AddFenToBoard(string? fen)
    {
        if (fen == null) return;
        try
        {
            Board.GenerateBoardWithFen(fen);
            StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
}