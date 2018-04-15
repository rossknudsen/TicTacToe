using System.Collections.Generic;

namespace TicTacToe.GameEngine
{
    /// <summary>
    /// Represents the current result/outcome of a game.
    /// </summary>
    public class GameResult
    {
        public GameResult(GameState gameState, PlayerToken? winningToken = null, IEnumerable<Square> winningDirection = null)
        {
            GameState = gameState;
            WinningToken = winningToken;
            WinningDirection = winningDirection;
        }

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState GameState { get; }

        /// <summary>
        /// If the current state is <see cref="GameState.Won"/> then this will contain the winning <see cref="PlayerToken"/>.
        /// Otherwise will be null.
        /// </summary>
        public PlayerToken? WinningToken { get; }

        /// <summary>
        /// If the current state is <see cref="GameState.Won"/> then this will contain the winning direction.
        /// Otherwise will be null.
        /// </summary>
        public IEnumerable<Square> WinningDirection { get; }
    }
}
