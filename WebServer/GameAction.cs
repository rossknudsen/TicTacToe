namespace WebServer
{
    internal class GameAction
    {
        public GameAction(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }
    }
}