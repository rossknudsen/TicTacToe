using System;
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
            
            try
            {
                // TODO get the correct game id from the request and check auth.  Is the gameId in the api resource path?  Where is the auth?
                var gameId = 0;

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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ServerErrorResponse();
            }
        }
    }
}
