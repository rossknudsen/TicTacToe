using System;
using System.Linq;
using TicTacToe.GameEngine;

namespace WebServer
{
    internal static class ComputerAi
    {
        internal static Square DetermineMove(Game game)
        {
            var blockOpponent = GetWinningMove(game.Board, game.HumanPlayer.PlayerToken);
            var winningMove = GetWinningMove(game.Board, game.ComputerPlayer.PlayerToken);
            if (winningMove != null)
            {
                return winningMove;
            }
            if (blockOpponent != null)
            {
                return blockOpponent;
            }
            return ChooseRandomSquare(game.Board);
        }

        private static Square GetWinningMove(Board board, PlayerToken token)
        {
            var directions = board.GetAllDirections()
                .Select(d => d.ToList());
            foreach (var direction in directions)
            {
                if (direction.Count(s => s.Token == token) == 2
                    && direction.Count(s => s.Token == PlayerToken.Empty) == 1)
                {
                    var emptySquare = direction.FirstOrDefault(s => s.Token == PlayerToken.Empty);
                    if (emptySquare != null)
                    {
                        return emptySquare;
                    }
                }
            }
            return null;
        }

        private static Square ChooseRandomSquare(Board board)
        {
            var emptySquares = board.Squares
                .Where(s => s.Token == PlayerToken.Empty)
                .ToList();

            var random = new Random();

            return emptySquares[random.Next(emptySquares.Count)];
        }
    }
}
