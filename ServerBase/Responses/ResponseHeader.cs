using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Responses
{
    /// <summary>
    /// Respresents the headers in a HTTP response
    /// </summary>
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

        /// <summary>
        /// The HTTP response code for this response
        /// </summary>
        public HttpResponseCode Code { get; }

        /// <summary>
        /// The human readable message associated with the response code
        /// </summary>
        public string ResponseText { get; }

        /// <summary>
        /// Parses a HTTP header in bytes.
        /// </summary>
        /// <param name="headerData">The header information in byte form.</param>
        /// <param name="header">The parsed <see cref="ResponseHeader"/> instance.</param>
        /// <returns>True if the header is successfully parsed.</returns>
        public static bool TryParseHeader(byte[] headerData, out ResponseHeader header)
        {
            // split the header data into strings representing each line of the header.
            // pass the first line and subsequent lines to the appropriate method for parsing.
            var lines = ConvertDataToLines(headerData);
            var headers = ParseHeaders(lines.Skip(1));
            var firstLine = ReadFirstLine(lines.First());

            // return a new response header instance containing the parsed results.
            header = new ResponseHeader(firstLine.code, firstLine.responseText, headers);
            return true;
        }

        private static (HttpResponseCode code, string responseText) ReadFirstLine(string line)
        {
            // we just split the first line using the space character.
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