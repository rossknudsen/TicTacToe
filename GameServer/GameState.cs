using TicTacToe.GameEngine;

namespace TicTacToe
{
    /// <summary>
    /// An object that ties an identifier to each game instance for storing
    /// in the <see cref="GameManager"/> and for transfer over the network.
    /// </summary>
    internal class GameState
    {
        public GameState(int id, Game game)
        {
            GameId = id;
            Game = game;
        }

        /// <summary>
        /// An unique id to represent this game.
        /// </summary>
        public int GameId { get; }

        /// <summary>
        /// The game to which this id relates.
        /// </summary>
        public Game Game { get; }
    }
}