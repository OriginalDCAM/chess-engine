﻿using System.Diagnostics;

namespace ChessEngine;

public class Board
{
    public HashSet<string> FenList { get; } = new();

    private Player CanMove { get; set; } = Player.White;

    public string? LastAddedFen { get; set; }

    public ulong[] Bitboard { get; } = new ulong[12];


    public Board()
    {
        GenerateBoardWithFen();
        GenerateBoardWithFen("8/5k2/3p4/1p1Pp2p/pP2Pp1P/P4P1K/8/8 b - - 99 50");
    }

    public void GenerateBoardWithFen(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq c6 0 2")
    {
        if (Bitboard.Any(bitboard => bitboard != 0)) Array.Clear(Bitboard);

        string[] fenParts = fen.Split(' ');
        
        string[] ranks = fenParts[0].Split('/');
        string startingColor = fenParts[1];
        
        if (ranks.Length > 8)
        {
            throw new FormatException("Invalid FEN: " + fen);
        }
        
        PlacePiecesOnBoard(ranks);
        CanMove = DecideWhoStartsFirst(startingColor);

        FenList.Add(fen);
        LastAddedFen = fen;
    }

    private void PlacePiecesOnBoard(string[] ranks)
    {
        for (var rank = 0; rank < 8; rank++)
        {
            var file = 0;
            foreach (var character in ranks[rank])
            {
                if (char.IsDigit(character))
                {
                    file += int.Parse(character.ToString());
                    continue;
                }

                var pieceIndex = Piece.GetPieceIndex(character);
                var squareIndex = rank * 8 + file;
                var pieceBit = 1UL << squareIndex;
                Bitboard[pieceIndex] |= pieceBit;
                file++;
            }
        }
    }

    private Player DecideWhoStartsFirst(string color)
    {
        return color == "w" ? Player.White : Player.Black;
    }

    public bool Move(int fromSquare, int toSquare, Player color)
    {
        if (color != CanMove) return false;
        var fromMask = 1UL << fromSquare;
        var toMask = 1UL << toSquare;
        
        var fromBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(fromSquare)); 
        var toBb = Piece.GetPieceIndex(GetPieceSymbolAtSquare(toSquare));
        

        Bitboard[fromBb] &= ~fromMask; // preform AND operation on the bitboard 
        Bitboard[fromBb] |= toMask; // preforms OR operation on the bitboard
        
        if (toBb != -1) Bitboard[toBb] ^= toMask; // this checks if the piece index is not -1 because that's an invalid index in the bb array. 
        
        CanMove = Player.White == CanMove ? Player.Black : Player.White;
        
        Console.WriteLine($"this player can now move: {CanMove}");
        return true;
    }

    public List<Move> GenerateLegalMoves()
    {
        var moves = new List<Move>();
        
        return moves;
    }

    public char GetPieceSymbolAtSquare(int squareIndex)
    {
        for (var pieceIndex = 0; pieceIndex < Bitboard.Length; pieceIndex++)
        {
            var bitboard = Bitboard[pieceIndex];
            var mask = 1UL << squareIndex;
            if ((bitboard & mask) != 0) return Piece.GetPieceSymbol(pieceIndex);
        }

        return '.';
    }

    public Player GetColorAtSquare(int squareIndex)
    {
        char pieceSymbol = GetPieceSymbolAtSquare(squareIndex);

        return char.IsUpper(pieceSymbol) ? Player.White : Player.Black;
    }
}