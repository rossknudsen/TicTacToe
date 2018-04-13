using System.Text;

namespace TicTacToe.Requests
{
    public class Request : MessageBase
    {
        private readonly RequestHeader _header;

        public Request(RequestHeader header, byte[] body = null) 
            : base(header, body)
        {
            _header = header;
        }

        public HttpMethod Method => _header.Method;

        public string Resource => _header.Resource;

        protected override string GenerateFirstLine()
        {
            return $"{Method.ToString()} {Resource} HTTP/1.0\n";
        }
    }
}