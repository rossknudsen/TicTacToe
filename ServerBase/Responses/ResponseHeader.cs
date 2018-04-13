using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Responses
{
    public class ResponseHeader : HeaderBase
    {
        public ResponseHeader(HttpResponseCode code, string responseText, Dictionary<string, string> headers)
        {
            Code = code;
            ResponseText = responseText;
            foreach (var header in headers)
            {
                this[header.Key.ToLower()] = header.Value;
            }
        }

        public HttpResponseCode Code { get; }

        public string ResponseText { get; }

        public static bool TryParseHeader(byte[] headerData, out ResponseHeader header)
        {
            var lines = ConvertDataToLines(headerData);
            var headers = ParseHeaders(lines.Skip(1));
            var firstLine = ReadFirstLine(lines.First());

            header = new ResponseHeader(firstLine.code, firstLine.responseText, headers);
            return true;
        }

        private static (HttpResponseCode code, string responseText) ReadFirstLine(string line)
        {
            var split = line.Split(new[] { ' ' }, 3);
            var responseText = split[2];
            if (!Enum.TryParse(split[1], out HttpResponseCode code))
            {
                throw new ArgumentException("Header Line is malformed.");
            }
            return (code, responseText);
        }
    }
}