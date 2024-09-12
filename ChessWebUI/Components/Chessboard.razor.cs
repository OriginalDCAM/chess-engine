using ChessEngine;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ChessWebUI.Components;

public partial class Chessboard : ComponentBase
{
    [Parameter] public required Board Board { get; set; }

    public List<int>? _visualizeAttackList { get; set; }

    private int _selectedSquare = -1; // Track selected square

    public Chessboard()
    {
        _visualizeAttackList = [];
    }

    private void OnSquareClick(int square)
    {
        if (_selectedSquare == -1) // No piece selected, select this one
        {
            if (Board.GetPieceSymbolAtSquare(square) != '.')
            {
                _selectedSquare = square;
                var color = Board.GetColorAtSquare(_selectedSquare);
                if (Board.CanMove != color) return;
                var moveGen = new MoveGen();
                var board = Board;
                List<Move> moves = moveGen.GenerateMoves(ref board, color);
                foreach (var move in moves)
                {
                    if (move.StartSquare == _selectedSquare)
                    {
                        _visualizeAttackList?.Add(move.TargetSquare);
                        StateHasChanged();
                    }
                    
                }
            }
        }
        else if (_selectedSquare == square)
        {
            _selectedSquare = -1;
            _visualizeAttackList?.Clear();
            StateHasChanged();
        }
        else // Piece already selected, move it
        {
            var color = Board.GetColorAtSquare(_selectedSquare);
            var move = new Move(_selectedSquare, square);
            Board.Move(move, color); // Make the move
            _selectedSquare = -1; // Deselect after move
            _visualizeAttackList.Clear();
            StateHasChanged();
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