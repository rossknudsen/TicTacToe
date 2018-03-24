using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.GameEngine
{
    public class Board
    {
        private readonly Square[,] _squares;

        internal Board()
        {
            _squares = new Square[3, 3];
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                    _squares[rowIndex, columnIndex] = new Square();
                }
            }
        }

        public IEnumerable<Square> Squares
        {
            get
            {
                // loop across rows first.
                for (var row = 0; row < 3; row++)
                {
                    for (var column = 0; column < 3; column++)
                    {
                        yield return _squares[row, column];
                    }
                }
            }
        }

        public int GetNumberOfTokensForPlayer(Player player)
        {
            return Squares.Count(s => s.Token == player.PlayerToken);
        }

        public GameResult GetGameResult()
        {
            // check all the possible winning scenarios.
            var winningDirection = GetAllDirections()
                .Select(d => d.ToList())
                .ToList()
                .FirstOrDefault(IsDirectionWinner);

            if (winningDirection != null)
            {
                var winningToken = winningDirection.First().Token;
                return new GameResult(GameState.Won, winningToken, winningDirection);
            }

            if (Squares.Any(s => s.Token == PlayerToken.Empty))
            {
                return new GameResult(GameState.Playing);
            }
            return new GameResult(GameState.Draw);
        }

        private bool IsDirectionWinner(IEnumerable<Square> direction)
        {
            var squares = direction.ToList();
            if (squares.Count(s => s.Token == PlayerToken.Circle) == 3)
            {
                return true;
            }
            if (squares.Count(s => s.Token == PlayerToken.Cross) == 3)
            {
                return true;
            }
            return false;
        }

        private IEnumerable<IEnumerable<Square>> GetAllDirections()
        {
            yield return GetRow(0);
            yield return GetRow(1);
            yield return GetRow(2);
            yield return GetColumn(0);
            yield return GetColumn(1);
            yield return GetColumn(2);
            yield return GetDiagonal(0);
            yield return GetDiagonal(1);
        }

        public IEnumerable<IEnumerable<Square>> GetRows()
        {
            yield return GetRow(0);
            yield return GetRow(1);
            yield return GetRow(2);
        }

        private IEnumerable<Square> GetRow(int rowNumber)
        {
            for (var columnIndex = 0; columnIndex < 3; columnIndex++)
            {
                yield return GetSquare(rowNumber, columnIndex);
            }
        }

        private IEnumerable<Square> GetColumn(int columnNumber)
        {
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                yield return GetSquare(rowIndex, columnNumber);
            }
        }

        private IEnumerable<Square> GetDiagonal(int index)
        {
            if (index == 0)
            {
                yield return GetSquare(0, 0);
                yield return GetSquare(1, 1);
                yield return GetSquare(2, 2);
            }
            else if (index == 1)
            {
                yield return GetSquare(2, 0);
                yield return GetSquare(1, 1);
                yield return GetSquare(0, 2);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public Square GetSquare(int row, int column)
        {
            if (row < 0 || 2 < row)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }
            if (column < 0 || 2 < column)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }
            return _squares[row, column];
        }
    }
}