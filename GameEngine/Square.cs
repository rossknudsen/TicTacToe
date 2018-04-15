namespace TicTacToe.GameEngine
{
    /// <summary>
    /// Represents a single square on the <see cref="Board"/> of a TicTacToe game.
    /// </summary>
    public class Square
    {
        internal Square()
        {
            Token = PlayerToken.Empty;
        }

        /// <summary>
        /// The <see cref="PlayerToken"/> this square contains.  Starts with <see cref="PlayerToken.Empty"/>
        /// </summary>
        public PlayerToken Token { get; internal set; }
    }
}