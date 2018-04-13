using System.Collections.Generic;

namespace TicTacToe.Responses
{
    public class RedirectResponse : Response
    {
        public RedirectResponse(string redirectUrl)
            : base(HttpResponseCode.Redirect, "Redirect", new Dictionary<string, string> { { "location", redirectUrl } })
        { }
    }
}