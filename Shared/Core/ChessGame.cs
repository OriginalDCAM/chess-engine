namespace ChessEngine.Core;

public class ChessGame
{
    public Board Board { get; set; }
    public Player CurrentPlayer { get; set; }
    
    public ChessGame()
    {
        Board = new Board();
        Board.Init();
        CurrentPlayer = Board.CanMove;
    }
    
    
    
}