namespace TicTacToe.GameEngine
{
    public class Square
    {
        internal Square()
        {
            Token = PlayerToken.Empty;
        }

        public PlayerToken Token { get; internal set; }
    }
}