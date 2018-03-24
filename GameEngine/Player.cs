namespace TicTacToe.GameEngine
{
    public class Player
    {
        internal Player(string name, PlayerToken token)
        {
            Name = name;
            PlayerToken = token;
        }

        public string Name { get; }

        public PlayerToken PlayerToken { get; }
    }
}