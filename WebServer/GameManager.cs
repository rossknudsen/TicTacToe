using TicTacToe.GameEngine;

namespace WebServer
{
    internal class GameManager
    {
        private readonly Game _game;

        public GameManager()
        {
            _game = Game.CreateGame();
        }

        internal Game ExecuteGameAction(int gameId, GameAction action)
        {
            // TODO lookup the game based on id.

            _game.PlaceToken(_game.HumanPlayer, action.Row, action.Column);

            // TODO make computer player move.

            return _game;
        }
    }
}