using System.Collections.Generic;
using System.Text;

namespace WebServer.Responses
{
    internal class ConstructedResponse : Response
    {
        private readonly string _body;

        public ConstructedResponse(HttpResponseCode responseCode, string responseText, Dictionary<string, string> headers, string body)
        {
            _body = body;
            ResponseCode = responseCode;
            ResponseText = responseText;
            foreach (var header in headers)
            {
                Headers[header.Key] = header.Value;
            }
        }

        public override HttpResponseCode ResponseCode { get; }

        public override string ResponseText { get; }

        protected override byte[] CreateResponseBody() => Encoding.ASCII.GetBytes(_body);
    }
}