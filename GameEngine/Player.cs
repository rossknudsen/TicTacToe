namespace TicTacToe.GameEngine
{
    /// <summary>
    /// Represents a single player in the game.
    /// </summary>
    public class Player
    {
        internal Player(string name, PlayerToken token)
        {
            Name = name;
            PlayerToken = token;
        }

        /// <summary>
        /// The name associated with the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="PlayerToken"/> that belongs to this player.
        /// </summary>
        public PlayerToken PlayerToken { get; }
    }
}