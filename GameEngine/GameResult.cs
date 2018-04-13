using System.Collections.Generic;

namespace TicTacToe.GameEngine
{
    public class GameResult
    {
        public GameResult(GameState gameState, PlayerToken? winningToken = null, IEnumerable<Square> winningDirection = null)
        {
            GameState = gameState;
            WinningToken = winningToken;
            WinningDirection = winningDirection;
        }

        public GameState GameState { get; }

        public PlayerToken? WinningToken { get; }

        public IEnumerable<Square> WinningDirection { get; }
    }
}
