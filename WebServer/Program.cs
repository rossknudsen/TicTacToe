using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebServer.Responses;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = "127.0.0.1";
            var port = 8080;
            var ep = new IPEndPoint(IPAddress.Parse(host), port);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(ep);
                socket.Listen(10); // TODO what does this do?

                while (true)
                {
                    using (var handler = socket.Accept())
                    {
                        var data = ReceiveData(handler);
                        var response = GenerateResponse(data);
                        SendResponse(handler, response);
                    }
                }
            }
        }

        private static Response GenerateResponse(byte[] data)
        {
            Response response;
            if (Request.TryParseRequest(data, out var request))
            {
                response = ProcessRequest(request);
            }
            else
            {
                Trace.WriteLine("Unable to parse request for " + Encoding.UTF8.GetString(data));
                return new BadRequestResponse();
            }
            return response;
        }

        private static byte[] ReceiveData(Socket handler)
        {
            // TODO handle requests that are longer than 1024 bytes long.
            var bytes = new byte[1024];
            var bytesCount = handler.Receive(bytes);
            var result = new byte[bytesCount];
            Array.Copy(bytes, result, bytesCount);
            return result;
        }

        private static void SendResponse(Socket handler, Response response)
        {
            handler.Send(response.ToBytes());
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static Response ProcessRequest(Request request)
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

        private static Response ProcessApiRequest(Request request)
        {
            // TODO implement handling of API requests.
            throw new NotImplementedException();
        }
    }
}
