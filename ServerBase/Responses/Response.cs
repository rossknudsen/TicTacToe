using System.Collections.Generic;

namespace TicTacToe.Responses
{
    /// <summary>
    /// Represents a HTTP response
    /// </summary>
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

        /// <summary>
        /// The HTTP response code for this response
        /// </summary>
        public HttpResponseCode ResponseCode { get; }

        /// <summary>
        /// The human readable message associated with the response code
        /// </summary>
        public string ResponseText { get; }

        protected override string GenerateFirstLine()
        {
            return $"HTTP/1.0 {(int) ResponseCode} {ResponseText}\n";
        }
    }
}