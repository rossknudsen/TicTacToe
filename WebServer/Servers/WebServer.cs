using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using TicTacToe.Requests;
using TicTacToe.Responses;

namespace TicTacToe.Servers
{
    public class WebServer : SocketServerBase
    {
        private readonly string _gameServerHost;
        private readonly int _gameServerPort;

        public WebServer(string host, int port, string gameServerHost, int gameServerPort) : base(host, port)
        {
            _gameServerHost = gameServerHost;
            _gameServerPort = gameServerPort;
        }

        protected override Response GenerateResponse(Request request)
        {
            // TODO we should implement a router to choose the handler for the request.
            if (request.Resource == ""
                || request.Resource == "/")
            {
                return ProcessRedirect(request);
            }
            else if (request.Resource.StartsWith("/api"))
            {
                return ProcessApiRequest(request);
            }
            else
            {
                return ProcessFileRequest(request);
            }
        }

        private static Response ProcessRedirect(Request request)
        {
            return new RedirectResponse("/index.html");
        }

        private static Response ProcessFileRequest(Request request)
        {
            var resourcePath = request.Resource;
            if (resourcePath.StartsWith("\\") || resourcePath.StartsWith("/"))
            {
                resourcePath = resourcePath.Substring(1);
            }
            resourcePath = Path.Combine(".\\wwwroot\\", resourcePath);

            if (!File.Exists(resourcePath))
            {
                return new NotFoundResponse();
            }

            try
            {
                return new OkResponse(new FileInfo(resourcePath));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new ServerErrorResponse();
            }
        }

        private Response ProcessApiRequest(Request request)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(_gameServerHost, _gameServerPort);

                if (!socket.Connected)
                {
                    // TODO do something here if we cannot connect.
                }

                Console.WriteLine($"Sending request to game server: {request.Method} {request.Resource}");
                socket.Send(request.ToBytes());

                Console.WriteLine("Receiving header from game server");
                var headerData = ReceiveHeaderData(socket);
                Console.WriteLine($"Received {headerData.Length} bytes from game server.");

                if (!ResponseHeader.TryParseHeader(headerData, out var header))
                {
                    return new ServerErrorResponse();
                }

                byte[] body = null;
                if (header.ContainsKey("content-length"))
                {
                    var bodyLength = Convert.ToInt32(header["content-length"]);

                    Console.WriteLine($"Receiving body from game server.  Expecting {bodyLength} bytes");
                    body = ReceiveBodyData(socket, bodyLength);
                    Console.WriteLine($"Finished receiving body from game server.  Received {body.Length} bytes.");
                }

                return new Response(header, body);
            }
        }
    }
}