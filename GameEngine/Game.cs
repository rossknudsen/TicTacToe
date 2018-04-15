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

        /// <summary>
        /// The <see cref="Board"/> for this game of TicTacToe.
        /// </summary>
        public Board Board { get; }

        /// <summary>
        /// The human player in this game of TicTacToe.
        /// </summary>
        public Player HumanPlayer { get; }

        /// <summary>
        /// The computer player in this game of TicTacToe.
        /// </summary>
        public Player ComputerPlayer { get; }

        /// <summary>
        /// The current result of this game of TicTacToe.
        /// </summary>
        public GameResult GameResult { get; private set; }

        public static Game CreateGame()
        {
            // TODO consider whether we should make some of this configurable or not.
            var humanPlayer = new Player("Human", PlayerToken.Cross);

            var computerPlayer = new Player("Computer", PlayerToken.Circle);

            return new Game(new Board(), humanPlayer, computerPlayer);
        }

        /// <summary>
        /// This method allows either player to place a token on the board.
        /// </summary>
        /// <param name="player">The player that is placing their token on the board.</param>
        /// <param name="row">The row index of the square that the player's token is to be placed.</param>
        /// <param name="column">The column index of the square that the player's token is to be placed.</param>
        public void PlaceToken(Player player, int row, int column)
        {
            var selectedSquare = Board.GetSquare(row,column);
            PlaceToken(player, selectedSquare);
        }

        /// <summary>
        /// This method allows either player to place a token on the board.
        /// </summary>
        /// <param name="player">The player that is placing their token on the board.</param>
        /// <param name="selectedSquare">The specified square that the player's token is to be placed.</param>
        public void PlaceToken(Player player, Square selectedSquare)
        {
            // if the provided square is not part of this game's board then throw an exception.
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

            // check that the game is not over.
            var gameOver = GameResult.GameState != GameState.Playing;
            if (gameOver)
            {
                throw new GameException("Game is over");
            }
            
            // check that the square is not already occupied.
            var squareOccupiedAlready = selectedSquare.Token != PlayerToken.Empty;
            if (squareOccupiedAlready)
            {
                throw new GameException("Square is occupied already.");
            }
            
            // finally update the square with the player's token and re-evaluate the game result.
            selectedSquare.Token = player.PlayerToken;
            GameResult = GetGameResult();
        }

        /// <summary>
        /// Recalculates the result of the current game board.
        /// </summary>
        /// <returns>The current result of the game.</returns>
        private GameResult GetGameResult()
        {
            // check all the possible winning scenarios.
            var winningDirection = Board.GetAllDirections()
                .Select(d => d.ToList())
                .ToList()
                .FirstOrDefault(Board.IsDirectionWinner);

            // check if there are any winning directions.
            if (winningDirection != null)
            {
                var winningToken = winningDirection.First().Token;
                return new GameResult(GameState.Won, winningToken, winningDirection);
            }

            // check if there are any squares that are still avaliable.
            if (Board.Squares.Any(s => s.Token == PlayerToken.Empty))
            {
                return new GameResult(GameState.Playing);
            }
            // otherwise the result is a draw.
            return new GameResult(GameState.Draw);
        }
    }
}
