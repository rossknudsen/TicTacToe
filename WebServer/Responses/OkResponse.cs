using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebServer.Responses
{
    internal class OkResponse : Response
    {
        private static readonly Dictionary<string, string> FileExtensionToContentTypeMap = new Dictionary<string, string>
        {
            { ".html", "text/html" },
            { ".jpg", "image/jpeg" },
            { ".js", "text/javascript" },
            { ".css", "text/css" }
        };

        private readonly byte[] _responseBytes;

        public OkResponse(string responseBody) : this(Encoding.UTF8.GetBytes(responseBody))
        {
            Headers["Content-Type"] = "application/json";
        }

        public OkResponse(FileInfo fileInfo) : this(File.ReadAllBytes(fileInfo.FullName))
        {
            Headers["Content-Type"] = FileExtensionToContentTypeMap[fileInfo.Extension];
        }

        public OkResponse(byte[] responseBytes)
        {
            _responseBytes = responseBytes;
            Headers["Content-Length"] = _responseBytes.Length.ToString();
            Headers["Cache-Control"] = "no-cache";
        }

        public override HttpResponseCode ResponseCode => HttpResponseCode.Ok;

        public override string ResponseText => "Ok";

        protected override byte[] CreateResponseBody() => _responseBytes;
    }
}