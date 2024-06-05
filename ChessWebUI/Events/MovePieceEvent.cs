using Microsoft.AspNetCore.Components;

namespace ChessWebUI.Events;

public class CustomEventArgs : EventArgs
{
    public int? from {get; set;}
    public int? to {get; set;}
}

[EventHandler("oncustomevent", typeof(CustomEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
    
}