using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using WebServer.Requests;
using WebServer.Responses;

namespace WebServer.Servers
{
    public class WebServer : SocketServerBase
    {
        public WebServer(string host, int port) : base(host, port) { }

        protected override Response GenerateResponse(byte[] data)
        {
            Response response;
            if (Request.TryParseRequest(data, out var request))
            {
                response = ProcessRequest(request);
            }
            else
            {
                Trace.WriteLine($"Unable to parse request for:\n {Encoding.UTF8.GetString(data)}");
                return new BadRequestResponse();
            }
            return response;
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
            // we should make a call to the game server to access the API.
            throw new NotImplementedException();
        }
    }
}