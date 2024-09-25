namespace ChessEngine.Utils;

public static class BitBoardHelper
{
    // Set a square in the bitboard (i.e., mark the square as occupied)
    public static void SetSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard |= (1UL << squareIndex); // Set the bit at squareIndex to 1
    }

    // Clear a square in the bitboard (i.e., mark the square as empty)
    public static void ClearSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard &= ~(1UL << squareIndex); // Set the bit at squareIndex to 0
    }

    // Toggle a square in the bitboard (i.e., flip its state)
    public static void ToggleSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard ^= (1UL << squareIndex); // Flip the bit at squareIndex
    }

    // Toggle two squares in the bitboard (i.e., flip the state of both squares)
    public static void ToggleSquares(ref ulong bitboard, int squareA, int squareB)
    {
        bitboard ^= (1UL << squareA); // Flip the bit at squareA
        bitboard ^= (1UL << squareB); // Flip the bit at squareB
    }

    // Check if the bitboard contains a set bit at the given square index
    public static bool ContainsSquare(ulong bitboard, int squareIndex)
    {
        return (bitboard & (1UL << squareIndex)) != 0; // Return true if the bit at squareIndex is 1
    }
}