using ChessEngine;
using Microsoft.AspNetCore.Components;
using ChessWebUI.Components;

namespace ChessWebUI.Components.Addons;

public partial class FenList : ComponentBase
{
    [Parameter] public required Action<string> AddFen { get; set; }
    
    [Parameter] public required string LastAddedFen { get; set; }
    
    [Parameter] public required HashSet<string> ListOfFens { get; set; }


    public required string InputFen { get; set; }

    private void OnInput(string value)
    {
        InputFen = value;
    }
}