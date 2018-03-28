using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Responses
{
    public abstract class Response
    {
        protected Response()
        {
            Headers = new Dictionary<string, string>();
        }

        public abstract HttpResponseCode ResponseCode { get; }

        public abstract string ResponseText { get; }

        public Dictionary<string, string> Headers { get; }

        public byte[] ToBytes()
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.Append($"HTTP/1.1 {(int) ResponseCode} {ResponseText}\n");
            foreach (var header in Headers)
            {
                responseBuilder.Append($"{header.Key}: {header.Value}\n");
            }
            responseBuilder.Append("\n");

            return Encoding.UTF8.GetBytes(responseBuilder.ToString())
                .Concat(CreateResponseBody())
                .Concat(Encoding.UTF8.GetBytes("\n"))
                .ToArray();
        }

        protected abstract byte[] CreateResponseBody();
    }

    internal abstract class NoBodyResponse : Response
    {
        protected override byte[] CreateResponseBody() => new byte[0];
    }
}