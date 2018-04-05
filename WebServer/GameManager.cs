using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.GameEngine;

namespace WebServer
{
    internal class GameManager
    {
        private readonly Dictionary<int, Game> _games;

        public GameManager()
        {
            _games = new Dictionary<int, Game>();
        }

        internal Game ExecuteGameAction(int gameId, GameAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var game = _games[gameId];
            if (game == null)
            {
                throw new ArgumentException(nameof(gameId));
            }

            // player move
            game.PlaceToken(game.HumanPlayer, action.Row, action.Column);

            // computer move if game not finished.
            if (game.GameResult.GameState == GameState.Playing)
            {
                game.PlaceToken(game.ComputerPlayer, ComputerAi.DetermineMove(game));
            }

            return game;
        }

        internal int CreateGame()
        {
            var newKey = 1;
            if (_games.Keys.Count > 0)
            {
                newKey = _games.Keys.Max() + 1;
            }
            _games[newKey] = Game.CreateGame();
            return newKey;
        }
    }
}