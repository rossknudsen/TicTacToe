using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TicTacToe.GameEngine;
using WebServer.Requests;
using WebServer.Responses;

namespace WebServer.Servers
{
    internal class GameServer : SocketServerBase
    {
        private readonly GameManager _gm;

        public GameServer(string host, int port) : base(host, port)
        {
            _gm = new GameManager();
        }

        protected override Response GenerateResponse(byte[] data)
        {
            if (!ApiRequest.TryParseRequest(data, out var request))
            {
                return new BadRequestResponse();
            }

            if (request.Resource == "/api/game/new" 
                && request.Method == HttpMethod.Get)
            {
                return HandleNewGameRequest(request);
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

        private Response HandleNewGameRequest(ApiRequest request)
        {
            var gameId = _gm.CreateGame();
            return new OkResponse(JsonConvert.SerializeObject(new { gameId = gameId })); 
        }

        private Response HandleGameActionRequest(ApiRequest request, int gameId)
        {
            try
            {
                var action = JsonConvert.DeserializeObject<GameAction>(request.Body);
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
    }
}
