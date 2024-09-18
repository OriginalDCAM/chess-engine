using ChessEngine;
using ChessWebUI.Components.Addons;
using Microsoft.AspNetCore.Components;


namespace ChessWebUI.Pages;

public partial class NewUi : ComponentBase
{
    private async void AddFenToBoard(string? fen)
    {
        if (fen == null) return;
        try
        {
            await SessionStorageAccessor.SetValueAsync("fen", fen);
            
            _chessBoard.InitializeBoard(fen);
            
            StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}