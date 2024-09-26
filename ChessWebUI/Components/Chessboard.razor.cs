using ChessEngine;
using ChessEngine.Core;
using ChessEngine.Structs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ChessWebUI.Components;

public partial class Chessboard : ComponentBase
{
    [Parameter] public required Board Board { get; set; }

    public List<int>? VisualizeAttackList { get; set; }

    private int _selectedSquare = -1; // Track selected square

    protected bool promotionPending = false;
    private int promotionSquare;

    public Chessboard()
    {
        VisualizeAttackList = [];
    }
    
    protected override void OnInitialized()
    {
        Board.OnPawnPromotion += HandlePawnPromotion;
    }

    private async void HandlePawnPromotion(int squareIndex, Player player)
    {
        promotionPending = true;
        promotionSquare = squareIndex;

        await OpenDialogAsync(squareIndex, player);
        
        StateHasChanged();
        
        Console.WriteLine("Pawn promotion pending");
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
                var moves = moveGen.GenerateMoves(Board, color);
                foreach (var move in moves)
                    if (move.StartSquare == _selectedSquare)
                    {
                        VisualizeAttackList?.Add(move.TargetSquare);
                        StateHasChanged();
                    }
            }
        }
        else if (_selectedSquare == square)
        {
            _selectedSquare = -1;
            VisualizeAttackList?.Clear();
            StateHasChanged();
        }
        else // Piece already selected, move it
        {
            var color = Board.GetColorAtSquare(_selectedSquare);
            var move = new Move(_selectedSquare, square);
            Board.Move(move, color); // Make the move
            _selectedSquare = -1; // Deselect after move
            VisualizeAttackList?.Clear();
            StateHasChanged();
        }
    }

}