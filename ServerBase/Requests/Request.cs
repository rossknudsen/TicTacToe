using System.Text;

namespace TicTacToe.Requests
{
    /// <summary>
    /// Represents a HTTP Request
    /// </summary>
    public class Request : MessageBase
    {
        private readonly RequestHeader _header;

        public Request(RequestHeader header, byte[] body = null) 
            : base(header, body)
        {
            _header = header;
        }

        /// <summary>
        /// The HTTP method or verb
        /// </summary>
        public HttpMethod Method => _header.Method;

        /// <summary>
        /// The requested resource
        /// </summary>
        public string Resource => _header.Resource;

        protected override string GenerateFirstLine()
        {
            return $"{Method.ToString()} {Resource} HTTP/1.0\n";
        }
    }
}