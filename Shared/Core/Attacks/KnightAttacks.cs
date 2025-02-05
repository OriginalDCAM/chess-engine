using ChessEngine.Structs;
using ChessEngine.Utils;

namespace ChessEngine.Core.Attacks;

public static class KnightAttacks
{
    private static readonly ulong[] KnightAttackTable = new ulong[64];

    static KnightAttacks()
    {
        for (int square = 0; square < 64; square++)
        {
            KnightAttackTable[square] = CalculateKnightAttacks(square);
        }
    }
    
    private static ulong CalculateKnightAttacks(int square)
    {
        var attacks = 0UL;
        int[] offsets = [10, 6, 15, 17, -10, -6, -17, -15];

        int startRank = square / 8;
        int startFile = square % 8;

        foreach (int offset in offsets)
        {
            int targetSquare = square + offset;
            if (targetSquare is < 0 or >= 64)continue;

            int targetRank = targetSquare / 8;
            int targetFile = targetSquare % 8;

            int rowDiff = Math.Abs(startRank - targetRank);
            int colDiff = Math.Abs(startFile - targetFile);

            if ((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2))
            {
                attacks |= 1UL << targetSquare;
            }
        }

        return attacks;
    }

    public static ulong GetKnightAttacks(int square)
    {
        return KnightAttackTable[square];
    }

    public static void GenerateKnightMoves(ref List<Move> moves, Board board, int startSquare, Player color)
    {
        ulong attacks = GetKnightAttacks(startSquare);

        attacks &= ~board.GetColorBitboard(color);

        while (attacks != 0)
        {
            int targetSquare = BitBoardHelper.GetLSBIndex(attacks);
            moves.Add(new Move(startSquare, targetSquare));
            attacks &= attacks - 1;
        }
    }
}