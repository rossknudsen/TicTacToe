using System;
using System.Linq;

namespace TicTacToe.GameEngine
{
    public class Game
    {
        private Game(Board board, Player humanPlayer, Player computerPlayer)
        {
            Board = board;
            HumanPlayer = humanPlayer;
            ComputerPlayer = computerPlayer;
            GameResult = GetGameResult();
        }

        public Board Board { get; }

        public Player HumanPlayer { get; }

        public Player ComputerPlayer { get; }

        public GameResult GameResult { get; private set; }

        public static Game CreateGame()
        {
            // TODO consider whether we should make some of this configurable or not.
            var humanPlayer = new Player("Human", PlayerToken.Cross);

            var computerPlayer = new Player("Computer", PlayerToken.Circle);

            return new Game(new Board(), humanPlayer, computerPlayer);
        }

        public void PlaceToken(Player player, int row, int column)
        {
            var selectedSquare = Board.GetSquare(row,column);
            PlaceToken(player, selectedSquare);
        }
        
        public void PlaceToken(Player player, Square selectedSquare)
        {
            if (!Board.Squares.Contains(selectedSquare))
            {
                throw new ArgumentException("Selected square is not part of the current game.");
            }

            // Check that the current turn is the player specified.
            var tokensForPlayer = Board.GetNumberOfTokensForPlayer(player);
            var tokensForOpponent =
                Board.GetNumberOfTokensForPlayer(player == HumanPlayer ? ComputerPlayer : HumanPlayer);

            if (tokensForPlayer > tokensForOpponent
                || (tokensForPlayer == tokensForOpponent && player != HumanPlayer))
            {
                throw new GameException("Its not your turn.");
            }

            var gameOver = GameResult.GameState != GameState.Playing;
            if (gameOver)
            {
                throw new GameException("Game is over");
            }
            
            var squareOccupiedAlready = selectedSquare.Token != PlayerToken.Empty;
            if (squareOccupiedAlready)
            {
                throw new GameException("Square is occupied already.");
            }
            
            selectedSquare.Token = player.PlayerToken;
            GameResult = GetGameResult();
        }

        private GameResult GetGameResult()
        {
            // check all the possible winning scenarios.
            var winningDirection = Board.GetAllDirections()
                .Select(d => d.ToList())
                .ToList()
                .FirstOrDefault(Board.IsDirectionWinner);

            if (winningDirection != null)
            {
                var winningToken = winningDirection.First().Token;
                return new GameResult(GameState.Won, winningToken, winningDirection);
            }

            if (Board.Squares.Any(s => s.Token == PlayerToken.Empty))
            {
                return new GameResult(GameState.Playing);
            }
            return new GameResult(GameState.Draw);
        }
    }
}
