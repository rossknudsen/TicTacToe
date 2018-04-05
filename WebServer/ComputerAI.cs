using System.Linq;
using TicTacToe.GameEngine;

namespace WebServer
{
    internal static class ComputerAi
    {
        internal static Square DetermineMove(Game game)
        {
            // Just takes the next available square.
            return game.Board.Squares
                .FirstOrDefault(s => s.Token == PlayerToken.Empty);
        }
    }
}
