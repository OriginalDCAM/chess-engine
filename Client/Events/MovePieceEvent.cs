using Microsoft.AspNetCore.Components;

namespace ChessWebUI.Events;

public class CustomEventArgs : EventArgs
{
    public int? from { get; set; }
    public int? to { get; set; }
}

[EventHandler("oncustomevent", typeof(CustomEventArgs),
    true, true)]
public static class EventHandlers
{
}