using ChessEngine;
using ChessEngine.Core;
using ChessEngine.Structs;
using ChessEngine.Utils;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ChessWebUI.Components;

public partial class Chessboard : ComponentBase
{
    [Parameter] public required Board Board { get; set; }

    private List<int>? VisualizeAttackList { get; set; } = [];

    private int _selectedSquare = -1; // Track selected square

    protected bool promotionPending = false;
    private int promotionSquare;

    protected override void OnInitialized()
    {
        Board.OnPawnPromotion += async (i, player) => await HandlePawnPromotion(i, player);
    }

    private async Task HandlePawnPromotion(int squareIndex, Player player)
    {
        promotionPending = true;
        promotionSquare = squareIndex;

        await OpenDialogAsync(squareIndex, player);

        StateHasChanged();

        Console.WriteLine("Pawn promotion pending");
    }

    private void OnSquareClick(int square)
    {
        if (Board.CanMove != Board.GetColorAtSquare(square) && _selectedSquare == -1) return;

        if (_selectedSquare == (int)Player.Empty) // No piece selected, select this one
        {
            if (Board.GetPieceSymbolAtSquare(square) != ' ')
            {
                _selectedSquare = square;
                var color = Board.GetColorAtSquare(_selectedSquare);
                if (Board.CanMove != color) return;
                var moveGen = new MoveGen();
                var moves = moveGen.GenerateAllLegalMoves(Board, color);
                foreach (var move in moves)
                    if (move.StartSquare == _selectedSquare)
                    {
                        VisualizeAttackList?.Add(move.TargetSquare);
                    }
            }
        }
        else if (_selectedSquare == square)
        {
            _selectedSquare = -1;
            VisualizeAttackList?.Clear();
        }
        else if (_selectedSquare != square && Board.GetColorAtSquare(square) == Board.CanMove)
        {
            VisualizeAttackList?.Clear();
            _selectedSquare = square;
            var color = Board.GetColorAtSquare(_selectedSquare);
            if (Board.CanMove != color) return;
            var moveGen = new MoveGen();
            var moves = moveGen.GenerateAllLegalMoves(Board, color);
            foreach (var move in moves)
                if (move.StartSquare == _selectedSquare)
                {
                    VisualizeAttackList?.Add(move.TargetSquare);
                }
        }
        else // Piece already selected, move it
        {
            var color = Board.GetColorAtSquare(_selectedSquare);
            var move = new Move(_selectedSquare, square);
            Board.MakeMove(move, color); // Make the move
            _selectedSquare = -1; // Deselect after move
            VisualizeAttackList?.Clear();
        }
        StateHasChanged();
    }
    
    
}