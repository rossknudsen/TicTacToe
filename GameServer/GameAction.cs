namespace TicTacToe
{
    /// <summary>
    /// Represents the human player action that is received over the network (i.e. as JSON).
    /// </summary>
    internal class GameAction
    {
        public GameAction(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// The row in which the token is to be placed.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// The column in which the token is to be placed.
        /// </summary>
        public int Column { get; }
    }
}