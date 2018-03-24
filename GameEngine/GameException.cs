using System;

namespace TicTacToe.GameEngine
{
    public class GameException : Exception
    {
        public GameException(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
