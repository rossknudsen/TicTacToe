using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Responses
{
    public abstract class Response
    {
        protected Response()
        {
            Headers = new Dictionary<string, string>();
        }

        public abstract HttpResponseCode ResponseCode { get; }

        public abstract string ResponseText { get; }

        public Dictionary<string, string> Headers { get; }

        public byte[] ToBytes()
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.Append($"HTTP/1.1 {(int) ResponseCode} {ResponseText}\n");
            foreach (var header in Headers)
            {
                responseBuilder.Append($"{header.Key}: {header.Value}\n");
            }
            responseBuilder.Append("\n");

            return Encoding.UTF8.GetBytes(responseBuilder.ToString())
                .Concat(CreateResponseBody())
                .Concat(Encoding.UTF8.GetBytes("\n"))
                .ToArray();
        }

        public static bool TryParseResponse(byte[] responseData, out Response response)
        {
            var responseString = Encoding.ASCII.GetString(responseData);

            var lines = responseString
                .Replace("\r", "")
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .ToList();

            var headers = new Dictionary<string, string>();
            var bodyBuilder = new StringBuilder();

            using (var enumerator = lines.GetEnumerator())
            {
                enumerator.MoveNext();

                ReadFirstLine(enumerator.Current, out var code, out var responseText);

                bool moveNext;
                while (true)
                {
                    moveNext = enumerator.MoveNext();

                    // if there is no next item we exit.
                    // if the current item is an empty string, we have finished the headers.
                    if (!moveNext
                        || enumerator.Current == "")
                    {
                        break;
                    }
                    ReadHeaderLine(enumerator.Current, headers);
                }
                if (moveNext)
                {
                    while (enumerator.MoveNext())
                    {
                        bodyBuilder.AppendLine(enumerator.Current);
                    }
                }

                response = new ConstructedResponse(code, responseText, headers, bodyBuilder.ToString());
                return true;
            }
        }

        private static void ReadFirstLine(string line, out HttpResponseCode code, out string responseText)
        {
            var split = line.Split(' ');
            responseText = split[2];
            if (!HttpResponseCode.TryParse(split[1], out code))
            {
                throw new ArgumentException("Header Line is malformed.");
            }
        }

        private static void ReadHeaderLine(string line, Dictionary<string, string> headers)
        {
            var split = line.Split(':');
            var key = split[0].Trim(' ');
            var value = split[1].Trim(' ');
            headers[key] = value;
        }

        protected abstract byte[] CreateResponseBody();
    }

    internal abstract class NoBodyResponse : Response
    {
        protected override byte[] CreateResponseBody() => new byte[0];
    }
}