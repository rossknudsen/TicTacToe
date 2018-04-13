using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.GameEngine;

namespace TicTacToe
{
    internal class GameManager
    {
        private readonly Dictionary<int, GameState> _games;

        public GameManager()
        {
            _games = new Dictionary<int, GameState>();
        }

        internal GameState ExecuteGameAction(int gameId, GameAction action)
        {
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

        internal GameState CreateGame()
        {
            var newKey = 1;
            if (_games.Keys.Count > 0)
            {
                newKey = _games.Keys.Max() + 1;
            }
            var gameState = new GameState(newKey, Game.CreateGame());
            _games[newKey] = gameState;
            return gameState;
        }
    }
}