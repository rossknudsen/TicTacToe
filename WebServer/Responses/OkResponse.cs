using System;
using System.IO;
using System.Text;

namespace WebServer.Responses
{
    internal class OkResponse : Response
    {
        private readonly byte[] _responseBytes;

        public OkResponse(string responseBody) : this(Encoding.UTF8.GetBytes(responseBody))
        {
            // TODO we will probably use this for API responses, we should set the contenttype.
        }

        public OkResponse(FileInfo fileInfo) : this(File.ReadAllBytes(fileInfo.FullName))
        {
            // we need to work out what content type we are dealing with.
            string contentType;
            switch (fileInfo.Extension)
            {
                case ".html":
                    contentType = "text/html";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                default:
                    throw new NotSupportedException($"Unknown file type: {fileInfo.Extension}");
            }
            Headers["Content-Type"] = contentType;
        }

        public OkResponse(byte[] responseBytes)
        {
            _responseBytes = responseBytes;
        }

        public override HttpResponseCode ResponseCode => HttpResponseCode.Ok;

        public override string ResponseText => "Ok";

        protected override byte[] CreateResponseBody() => _responseBytes;
    }
}