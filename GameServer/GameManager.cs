using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.GameEngine;

namespace TicTacToe
{
    /// <summary>
    /// This class manages the various TicTacToe games that various players may be playing concurrently
    /// and coordinates the instructions received from those players to the correct game.
    /// </summary>
    internal class GameManager
    {
        private readonly Dictionary<int, GameState> _games;

        public GameManager()
        {
            _games = new Dictionary<int, GameState>();
        }

        /// <summary>
        /// An action has been received from a player and should be executed against the correct game
        /// instance.  The resulting <see cref="GameState"/> is then returned and updated in the internal
        /// store.
        /// </summary>
        /// <param name="gameId">The id of the game to which the action should be applied.</param>
        /// <param name="action">The action to perform by the player.</param>
        /// <returns>The resulting <see cref="GameState"/></returns>
        internal GameState ExecuteGameAction(int gameId, GameAction action)
        {
            // validate our inputs
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var state = _games[gameId];
            if (state == null)
            {
                throw new ArgumentException(nameof(gameId));
            }

            // player move
            var game = state.Game;
            game.PlaceToken(game.HumanPlayer, action.Row, action.Column);

            // computer move if game not finished.
            if (game.GameResult.GameState == TicTacToe.GameEngine.GameState.Playing)
            {
                game.PlaceToken(game.ComputerPlayer, ComputerAi.DetermineMove(game));
            }

            return state;
        }

        /// <summary>
        /// This creates a new TicTacToe game.
        /// </summary>
        /// <returns></returns>
        internal GameState CreateGame()
        {
            // find an id for the new game.
            var newKey = 1;
            if (_games.Keys.Count > 0)
            {
                newKey = _games.Keys.Max() + 1;
            }
            // create a new game state object and store it in the internal dictionary, indexed by the id.
            var gameState = new GameState(newKey, Game.CreateGame());
            _games[newKey] = gameState;
            // return the game state.
            return gameState;
        }
    }
}