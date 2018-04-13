using System.Collections.Generic;

namespace TicTacToe.Responses
{
    public class Response : MessageBase
    {
        public Response(ResponseHeader header, byte[] body = null)
            : this(header.Code, header.ResponseText, header, body)
        { }

        public Response(HttpResponseCode code, string responseText, Dictionary<string, string> headers, byte[] body = null)
            : base(headers, body)
        {
            ResponseCode = code;
            ResponseText = responseText;
        }

        public virtual HttpResponseCode ResponseCode { get; }

        public virtual string ResponseText { get; }

        protected override string GenerateFirstLine()
        {
            return $"HTTP/1.0 {(int) ResponseCode} {ResponseText}\n";
        }
    }
}