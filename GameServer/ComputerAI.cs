using System;
using System.Linq;
using TicTacToe.GameEngine;

namespace TicTacToe
{
    /// <summary>
    /// This class implements the AI for the computer player.
    /// </summary>
    internal static class ComputerAi
    {
        /// <summary>
        /// This single method determines which square the computer player will choose based
        /// on the current board.
        /// </summary>
        /// <param name="game">The current game.</param>
        /// <returns>An empty square on the current board representing the computer player's move.</returns>
        internal static Square DetermineMove(Game game)
        {
            var blockOpponent = GetWinningMove(game.Board, game.HumanPlayer.PlayerToken);
            var winningMove = GetWinningMove(game.Board, game.ComputerPlayer.PlayerToken);
            if (winningMove != null)
            {
                // if there is a move in which the computer player will win, then choose that.
                return winningMove;
            }
            if (blockOpponent != null)
            {
                // otherwise can we block the other player from winning, then choose that.
                return blockOpponent;
            }
            // otherwise choose randomly from the remaining squares.
            return ChooseRandomSquare(game.Board);
        }

        /// <summary>
        /// This method finds the first direction on the given <see cref="Board"/> that results
        /// in a winning play for the specified <see cref="PlayerToken"/>.
        /// </summary>
        /// <param name="board">The current board.</param>
        /// <param name="token">The token to check for winning moves for.</param>
        /// <returns></returns>
        private static Square GetWinningMove(Board board, PlayerToken token)
        {
            // obtain a list of all the possible winning directions.
            var directions = board.GetAllDirections()
                .Select(d => d.ToList());
            // loop through each of the directions.
            foreach (var direction in directions)
            {
                // if there are two squares with mathing tokens and one empty square, then
                // this is a winning direction.
                if (direction.Count(s => s.Token == token) == 2
                    && direction.Count(s => s.Token == PlayerToken.Empty) == 1)
                {
                    // find the empty square and return it.
                    var emptySquare = direction.FirstOrDefault(s => s.Token == PlayerToken.Empty);
                    if (emptySquare != null)
                    {
                        return emptySquare;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a random square from the remaining empty squares.
        /// </summary>
        /// <param name="board">The current board.</param>
        private static Square ChooseRandomSquare(Board board)
        {
            // find all the empty squares.
            var emptySquares = board.Squares
                .Where(s => s.Token == PlayerToken.Empty)
                .ToList();

            var random = new Random();

            // return a random one from the empty squares.
            return emptySquares[random.Next(emptySquares.Count)];
        }
    }
}
