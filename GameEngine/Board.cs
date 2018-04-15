using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.GameEngine
{
    /// <summary>
    /// This object represents the Board of a game of TicTacToe.
    /// </summary>
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

        /// <summary>
        /// Returns each <see cref="Square"/> on the board row by row, left to right.
        /// </summary>
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

        /// <summary>
        /// Gets the count of <see cref="PlayerToken"/>s for the specified <see cref="Player"/>
        /// on the board.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that you wish to count the tokens of.</param>
        /// <returns>The count of tokens belonging to the <see cref="Player"/>.</returns>
        public int GetNumberOfTokensForPlayer(Player player)
        {
            return Squares.Count(s => s.Token == player.PlayerToken);
        }

        /// <summary>
        /// Checks to see if the given direction results in a win.  I.e. they are three of the same
        /// token in the direction given.
        /// </summary>
        /// <param name="direction">A collection of <see cref="Square"/>s in line</param>
        /// <returns>True if there are three tokens of the same kind in the specified direction.</returns>
        internal bool IsDirectionWinner(IEnumerable<Square> direction)
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

        /// <summary>
        /// Returns all the possible winning directions on the board.
        /// </summary>
        public IEnumerable<IEnumerable<Square>> GetAllDirections()
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

        /// <summary>
        /// Returns just the rows on the board.
        /// </summary>
        public IEnumerable<IEnumerable<Square>> GetRows()
        {
            yield return GetRow(0);
            yield return GetRow(1);
            yield return GetRow(2);
        }

        /// <summary>
        /// Gets the specified row by index.
        /// </summary>
        /// <param name="rowNumber">The index of the row desired (zero-based).</param>
        /// <returns>The desired row.</returns>
        private IEnumerable<Square> GetRow(int rowNumber)
        {
            for (var columnIndex = 0; columnIndex < 3; columnIndex++)
            {
                yield return GetSquare(rowNumber, columnIndex);
            }
        }

        /// <summary>
        /// Gets the specified column by index.
        /// </summary>
        /// <param name="rowNumber">The index of the column desired (zero-based).</param>
        /// <returns>The desired column.</returns>
        private IEnumerable<Square> GetColumn(int columnNumber)
        {
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                yield return GetSquare(rowIndex, columnNumber);
            }
        }

        /// <summary>
        /// Gets the specified diagonal by index.
        /// </summary>
        /// <param name="rowNumber">The index of the diagonal desired (zero-based).</param>
        /// <returns>The desired diagonal.</returns>
        /// <remarks>Note that there are only two diagonals.</remarks>
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

        /// <summary>
        /// Gets the specified square based on the row and column indexes provided.
        /// </summary>
        /// <param name="row">The row index of the square desired.</param>
        /// <param name="column">The column index of the square desired.</param>
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