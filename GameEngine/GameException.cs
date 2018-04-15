using System;

namespace TicTacToe.GameEngine
{
    /// <summary>
    /// An exception that is thrown when a <see cref="Player"/> attempts
    /// an invalid move.
    /// </summary>
    public class GameException : Exception
    {
        public GameException(string message) : base(message)
        { }
    }
}
