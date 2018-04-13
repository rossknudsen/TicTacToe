using System.Collections.Generic;

namespace TicTacToe.Responses
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
            :base(HttpResponseCode.NotFound, "Not Found", new Dictionary<string, string>())
        {
        }
    }
}