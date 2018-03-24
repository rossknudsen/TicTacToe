using System;
using System.Linq;
using TicTacToe.GameEngine;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = Game.CreateGame();

            while (true)
            {
                DrawBoard(game);

                DrawInstructions();
                
                if (game.Board.GetGameResult().GameState == GameState.Playing)
                {
                    if (!HumanPlayerTurn(game))
                    {
                        continue;
                    }
                }
                else
                {
                    break;
                }
                
                if (game.Board.GetGameResult().GameState == GameState.Playing)
                {
                    ComputerPlayerTurn(game);
                }
                else
                {
                    break;
                }
            }

            DrawBoard(game);
            PrintGameResult(game);
        }

        private static void PrintGameResult(Game game)
        {
            var result = game.Board.GetGameResult();
            switch (result.GameState)
            {
                case GameState.Draw:
                    Console.WriteLine("\nGame was a draw.");
                    break;

                case GameState.Won:
                    if (result.WinningToken == game.HumanPlayer.PlayerToken)
                    {
                        Console.WriteLine("You won!");
                    }
                    else
                    {
                        Console.WriteLine("You lost!");
                    }
                    break;

                case GameState.Playing:
                default:
                    throw new Exception("Don't know how this happened.");
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        private static bool HumanPlayerTurn(Game game)
        {
            var input = Console.ReadLine();
            
            if (TryParseInput(input, out var move))
            {
                try
                {
                    game.PlaceToken(game.HumanPlayer, move.Item1, move.Item2);
                }
                catch (GameException)
                {
                    Console.WriteLine("\nSorry that doesn't appear to be a valid move.\n");
                    return false;
                }
            }
            else
            {
                // display error and redraw board.
                Console.WriteLine("\nSorry I couldn't understand that input.\n");
                return false;
            }
            return true;
        }

        private static void ComputerPlayerTurn(Game game)
        {
            // TODO create a better AI.
            var square = game.Board.Squares.FirstOrDefault(s => s.Token == PlayerToken.Empty);
            game.PlaceToken(game.ComputerPlayer, square);
        }

        private static void DrawBoard(Game game)
        {
            Console.WriteLine("\n-------------");
            foreach (var row in game.Board.GetRows())
            {
                Console.Write("| ");
                foreach (var square in row)
                {
                    switch (square.Token)
                    {
                        case PlayerToken.Empty:
                            Console.Write("  | ");
                            break;
                        case PlayerToken.Cross:
                            Console.Write("X | ");
                            break;
                        case PlayerToken.Circle:
                            Console.Write("O | ");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                Console.WriteLine("\n-------------");
            }
        }

        private static void DrawInstructions()
        {
            Console.WriteLine("");
            Console.WriteLine("Please Enter your move in the form 'row column' (zero indexed):");
        }

        private static bool TryParseInput(string input, out Tuple<int, int> move)
        {
            var splitInput = input.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitInput.Length != 2)
            {
                move = null;
                return false;
            }

            if (!int.TryParse(splitInput[0], out var row)
                || !int.TryParse(splitInput[1], out var column))
            {
                move = null;
                return false;
            }

            if (row < 0 || 2 < row
                || column < 0 || 2 < column)
            {
                move = null;
                return false;
            }
            move = new Tuple<int, int>(row, column);
            return true;
        }
    }
}
