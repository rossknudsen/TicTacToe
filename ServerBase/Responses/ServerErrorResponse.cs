using System.Collections.Generic;

namespace TicTacToe.Responses
{
    public class ServerErrorResponse : Response
    {
        public ServerErrorResponse()
            : base(HttpResponseCode.ServerError, "Internal Server Error", new Dictionary<string, string>())
        { }
    }
}