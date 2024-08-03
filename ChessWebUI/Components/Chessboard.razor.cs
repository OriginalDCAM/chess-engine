using ChessEngine;
using Microsoft.AspNetCore.Components;

namespace ChessWebUI.Components;

public partial class Chessboard : ComponentBase
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
        else if (_selectedSquare == square) _selectedSquare = -1;
        else // Piece already selected, move it
        {
            var color = Board.GetColorAtSquare(_selectedSquare);
            var move = new Move(_selectedSquare, square);
            
            Board.Move(move, color); // Make the move
            _selectedSquare = -1; // Deselect after move
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