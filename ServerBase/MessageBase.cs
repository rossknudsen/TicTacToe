using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public abstract class MessageBase
    {
        private const string ContentLengthHeaderKey = "content-length";

        protected MessageBase(Dictionary<string, string> headers, byte[] body = null)
        {
            Headers = headers;
            Body = body;
        }

        public Dictionary<string, string> Headers { get; }

        public byte[] Body { get; }

        public byte[] ToBytes()
        {
            if (Body != null && Body.Length > 0)
            {
                Headers[ContentLengthHeaderKey] = Body.Length.ToString();
            }
            else
            {
                Headers.Remove(ContentLengthHeaderKey);
            }
            var bodyData = Body ?? new byte[0];
            var responseBuilder = new StringBuilder();
            responseBuilder.Append(GenerateFirstLine());
            foreach (var header in Headers)
            {
                responseBuilder.Append($"{header.Key}: {header.Value}\n");
            }
            responseBuilder.Append("\n");
            return Encoding.UTF8.GetBytes(responseBuilder.ToString())
                .Concat(bodyData)
                .ToArray();
        }

        protected abstract string GenerateFirstLine();
    }
}