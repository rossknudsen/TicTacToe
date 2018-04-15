using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using TicTacToe.Requests;
using TicTacToe.Responses;

namespace TicTacToe
{
    /// <summary>
    /// This class implements a basic webserver delivering static files and acting as a proxy for game API requests.
    /// </summary>
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
                // if the requested resouce is the root, we redirect the client to index.html.
                return ProcessRedirect(request);
            }
            else if (request.Resource.StartsWith("/api"))
            {
                // if the requested resource is under the /api path, we proxy to the game server.
                return ProcessApiRequest(request);
            }
            else
            {
                // we assume anything else is a request for a static file.
                return ProcessFileRequest(request);
            }
        }

        private static Response ProcessRedirect(Request request)
        {
            return new RedirectResponse("/index.html");
        }

        private static Response ProcessFileRequest(Request request)
        {
            // we determine the local file path for the requested resource.
            var resourcePath = request.Resource;
            if (resourcePath.StartsWith("\\") || resourcePath.StartsWith("/"))
            {
                resourcePath = resourcePath.Substring(1);
            }
            resourcePath = Path.Combine(".\\wwwroot\\", resourcePath);

            // if the file doesn't exist for the path, then we return a not found response.
            if (!File.Exists(resourcePath))
            {
                return new NotFoundResponse();
            }

            try
            {
                // otherwise we create a response containing the requested resource.
                return new OkResponse(new FileInfo(resourcePath));
            }
            catch (Exception e)
            {
                // log errors and send a server error response if we get to here.
                Trace.WriteLine(e);
                return new ServerErrorResponse();
            }
        }

        /// <summary>
        /// This method makes a call to the game server and proxies the response back to the client.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Response ProcessApiRequest(Request request)
        {
            // create a new socket that we can contact the game server on.
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(_gameServerHost, _gameServerPort);

                if (!socket.Connected)
                {
                    // TODO do something here if we cannot connect.
                }

                // send a the request to the game server.
                Console.WriteLine($"Sending request to game server: {request.Method} {request.Resource}");
                socket.Send(request.ToBytes());

                // read the response header from the game server.
                Console.WriteLine("Receiving header from game server");
                var headerData = ReceiveHeaderData(socket);
                Console.WriteLine($"Received {headerData.Length} bytes from game server.");

                // parse the response header from the data received.
                if (!ResponseHeader.TryParseHeader(headerData, out var header))
                {
                    return new ServerErrorResponse();
                }

                // read the body data from the game server.
                byte[] body = null;
                if (header.ContainsKey("content-length"))
                {
                    var bodyLength = Convert.ToInt32(header["content-length"]);

                    Console.WriteLine($"Receiving body from game server.  Expecting {bodyLength} bytes");
                    body = ReceiveBodyData(socket, bodyLength);
                    Console.WriteLine($"Finished receiving body from game server.  Received {body.Length} bytes.");
                }

                // send a response back to the client using the data received from the game server.
                return new Response(header, body);
            }
        }
    }
}