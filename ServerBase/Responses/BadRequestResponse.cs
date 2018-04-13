using System.Collections.Generic;

namespace TicTacToe.Responses
{
    public class BadRequestResponse : Response
    {
        public BadRequestResponse() 
            : base(HttpResponseCode.BadRequest, "Bad Request", new Dictionary<string, string>())
        {
        }
    }
}