using ChessEngine.Structs;

namespace ChessEngine.Utils;

public static class BoardHelper
{
    public static int GetRankPosition(int square) => square / 8;
    
    public static string GetRankPositionPretty(int position) => (GetRankPosition(position) + 1).ToString();

    public static int GetFilePosition(int square) => square % 8;

    public static string ConvertSquareToSan(int square)
    {
        // Reverse
        string[] file = { "a", "b", "c", "d", "e", "f", "g", "h" };
        
        return $"{file[GetFilePosition(square)]}{8 - GetRankPosition(square)}";
    }

    public static string GetFileString(int square)
    {
        string[] file = { "a", "b", "c", "d", "e", "f", "g", "h" };
        
        return file[GetFilePosition(square)];
    }
    
    public static string ConvertMoveToSan(Move move)
    {
        return $"{ConvertSquareToSan(move.TargetSquare)}";
    }
    
    
    public static bool IsPromotion(int moveTargetSquare, Player player)
    {
        int rank = moveTargetSquare / 8;

        Console.WriteLine($"Rank: {rank}");

        return player switch
        {
            Player.White when rank == 0 => true,
            Player.Black when rank == 7 => true,
            _ => false
        }; 
    }
    
    

    
}