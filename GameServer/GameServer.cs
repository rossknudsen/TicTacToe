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
    internal class GameServer : SocketServerBase
    {
        private readonly GameManager _gm;

        public GameServer(string host, int port) : base(host, port)
        {
            _gm = new GameManager();
        }

        protected override Response GenerateResponse(Request request)
        {
            Console.WriteLine($"Received request for {request.Method.ToString()} {request.Resource}");

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
            return new BadRequestResponse();
        }

        private bool TryMatchGameActionsRoute(string resourcePath, out int gameId)
        {
            var match = Regex.Match(resourcePath, @"/api/game/(\d*)/actions");

            if (match.Success)
            {
                gameId = Convert.ToInt32(match.Groups[1].Value);
                return true;
            }

            gameId = -1;
            return false;
        }

        private Response HandleNewGameRequest(Request request)
        {
            var gameState = _gm.CreateGame();
            return new OkResponse(JsonConvert.SerializeObject(gameState)); 
        }

        private Response HandleGameActionRequest(Request request, int gameId)
        {
            try
            {
                var body = Encoding.ASCII.GetString(request.Body);
                var action = JsonConvert.DeserializeObject<GameAction>(body);
                if (action == null)
                {
                    Console.WriteLine($"Could not deserialize body into Game Action\n{body}");
                    return new BadRequestResponse();
                }

                var resultingGame = _gm.ExecuteGameAction(gameId, action);

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

        protected override void OnClosingConnection(EndPoint handlerRemoteEndPoint)
        {
            base.OnClosingConnection(handlerRemoteEndPoint);

            Console.WriteLine($"Game server closing connection to {handlerRemoteEndPoint}");
        }
    }
}
