using TicTacToe.GameEngine;

namespace TicTacToe
{
    internal class GameState
    {
        public GameState(int id, Game game)
        {
            GameId = id;
            Game = game;
        }

        public int GameId { get; }

        public Game Game { get; }
    }
}