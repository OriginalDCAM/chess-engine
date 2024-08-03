using ChessEngine;
using Microsoft.AspNetCore.Components;

namespace ChessWebUI.Pages;

public partial class NewUi : ComponentBase
{
    
    private void AddFenToBoard(string? fen)
    {
        if (fen == null) return;
        try
        {
            _chessBoard.GenerateBoardWithFen(fen);
            StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
}