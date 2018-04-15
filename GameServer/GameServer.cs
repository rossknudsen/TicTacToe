using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TicTacToe.GameEngine;
using TicTacToe.Requests;
using TicTacToe.Responses;

namespace TicTacToe
{
    /// <summary>
    /// This class acts as the game server.  Deriving from <see cref="SocketServerBase"/> it implements
    /// the logic to parse the HTTP requests from players and marshall the actions to the 
    /// <see cref="GameManager"/> for execution, returning the results as a HTTP response.
    /// </summary>
    internal class GameServer : SocketServerBase
    {
        private readonly GameManager _gm;

        public GameServer(string host, int port) : base(host, port)
        {
            // create an instance of the game manager.
            _gm = new GameManager();
        }

        protected override Response GenerateResponse(Request request)
        {
            Console.WriteLine($"Received request for {request.Method.ToString()} {request.Resource}");

            // check if the request is for the new game endpoint or the game action endpoint.
            if (request.Resource == "/api/game/new"
                && request.Method == HttpMethod.Get)
            {
                var response = HandleNewGameRequest(request);
                Console.WriteLine($"Returning new game response to webserver. {response.ResponseCode} {response.ResponseText} (body {response.Body.Length} bytes)");
                
                return response;
            }
            else if (TryMatchGameActionsRoute(request.Resource, out var gameId)
                     && request.Method == HttpMethod.Post)
            {
                return HandleGameActionRequest(request, gameId);
            }
            // if we don't match a known endpoint then we return a bad request response.
            return new BadRequestResponse();
        }

        /// <summary>
        /// This method checks to see if the provided resource path matches the game
        /// action endpoint.
        /// </summary>
        /// <param name="resourcePath">The path of the requested resource.</param>
        /// <param name="gameId">Returns the id of the game that the resource path is referencing
        /// if the result is successful.</param>
        /// <returns>True if the resource path matches the game action endpoint.</returns>
        private bool TryMatchGameActionsRoute(string resourcePath, out int gameId)
        {
            var match = Regex.Match(resourcePath, @"/api/game/(\d*)/actions");

            // if the regex matches then get the id value from the match and convert to an int.
            if (match.Success)
            {
                gameId = Convert.ToInt32(match.Groups[1].Value);
                return true;
            }

            // otherwise return false and set the game id to a negative number, which is an 
            // invalid key for game manager instance.
            gameId = -1;
            return false;
        }

        /// <summary>
        /// This method asks the <see cref="GameManager"/> to create a new game and then
        /// serializes the resulting <see cref="GameState"/> and creates an 
        /// <see cref="OkResponse"/> containing the state.
        /// </summary>
        /// <param name="request">The HTTP request object.</param>
        /// <returns>A HTTP response containing the new game state.</returns>
        private Response HandleNewGameRequest(Request request)
        {
            var gameState = _gm.CreateGame();
            return new OkResponse(JsonConvert.SerializeObject(gameState)); 
        }

        /// <summary>
        /// This method handles the API request to perform an action on a specified game.
        /// </summary>
        /// <param name="request">The API request from the user.</param>
        /// <param name="gameId">The id of the game that the action is intended for.</param>
        /// <returns>A HTTP response containing the resulting <see cref="GameState"/>
        /// or an error response on failure.</returns>
        private Response HandleGameActionRequest(Request request, int gameId)
        {
            try
            {
                // use the Newtonsoft.Json library to deserialize the JSON in the body of the request.
                var body = Encoding.ASCII.GetString(request.Body);
                var action = JsonConvert.DeserializeObject<GameAction>(body);

                // if there was a problem deserializing, then we'll get a null result.
                if (action == null)
                {
                    Console.WriteLine($"Could not deserialize body into Game Action\n{body}");
                    return new BadRequestResponse();
                }

                // pass the action and game id to the game manager to action.
                var resultingGame = _gm.ExecuteGameAction(gameId, action);

                // serialize the resulting game state using the Newtonsoft.Json library and
                // return it in a reponse.
                var json = JsonConvert.SerializeObject(resultingGame);
                return new OkResponse(json);
            }
            catch (JsonException e)
            {
                Console.WriteLine(e);
                return new BadRequestResponse();
            }
            catch (GameException e)
            {
                Console.WriteLine(e);
                return new BadRequestResponse();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return new BadRequestResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ServerErrorResponse();
            }
        }
    }
}
