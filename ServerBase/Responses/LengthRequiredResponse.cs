using System.Collections.Generic;

namespace TicTacToe.Responses
{
    internal class LengthRequiredResponse : Response
    {
        public LengthRequiredResponse()
            : base(HttpResponseCode.LengthRequired, "Length Required", new Dictionary<string, string>())
        { }
    }
}
