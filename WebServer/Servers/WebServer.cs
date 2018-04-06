using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WebServer.Requests;
using WebServer.Responses;

namespace WebServer.Servers
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

        protected override Response GenerateResponse(byte[] data)
        {
            Response response;
            if (Request.TryParseRequest(data, out var request))
            {
                response = ProcessRequest(request, data);
            }
            else
            {
                Trace.WriteLine($"Unable to parse request for:\n {Encoding.UTF8.GetString(data)}");
                return new BadRequestResponse();
            }
            return response;
        }

        private Response ProcessRequest(Request request, byte[] requestData)
        {
            // TODO we should implement a router to choose the handler for the request.
            if (request.Resource == ""
                || request.Resource == "/")
            {
                return ProcessRedirect(request);
            }
            else if (request.Resource.StartsWith("/api"))
            {
                return ProcessApiRequest(requestData);
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

        private Response ProcessApiRequest(byte[] requestData)
        {
            // we should make a call to the game server to access the API.

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(_gameServerHost, _gameServerPort);

                if (!socket.Connected)
                {
                    // TODO do something here if we cannot connect.
                }

                socket.Send(requestData); // TODO we could try receiving a request object
                // as a method parameter and then call a ToBytes() method on it to
                // reconstruct the data stream.

                var responseData = new byte[0];
                while (responseData.Length == 0)
                {
                    responseData = ReceiveData(socket);
                }

                if (Response.TryParseResponse(responseData, out var response))
                {
                    return response;
                }
                return new ServerErrorResponse();
            }
        }
    }
}