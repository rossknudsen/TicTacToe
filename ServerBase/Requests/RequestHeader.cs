using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Requests
{
    /// <summary>
    /// Represents the header in a HTTP request
    /// </summary>
    public class RequestHeader : HeaderBase
    {
        private RequestHeader(HttpMethod method, string requestedResource, Dictionary<string, string> headers)
        {
            Method = method;
            Resource = requestedResource;
            foreach (var header in headers)
            {
                Add(header.Key.ToLower(), header.Value);
            }
        }

        /// <summary>
        /// The HTTP method or verb
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// The requested resource
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// Parses a header from raw bytes
        /// </summary>
        /// <param name="headerData">The raw data of a HTTP request</param>
        /// <param name="header">The header parsed into a <see cref="RequestHeader"/> instance.</param>
        /// <returns>True if the header is correctly decoded.</returns>
        public static bool TryParseHeader(byte[] headerData, out RequestHeader header)
        {
            // transform the binary data into a list of strings representing each line of the request
            // header.  Then pass the first line and the remaining lines off for parsing respectively.
            var lines = ConvertDataToLines(headerData);
            var headers = ParseHeaders(lines.Skip(1));
            var firstLine = ReadFirstLine(lines.First());

            // create a new request header instance using the parsed results.
            header = new RequestHeader(firstLine.Item1, firstLine.Item2, headers);
            return true;
        }

        private static Tuple<HttpMethod, string> ReadFirstLine(string line)
        {
            // split the first line using space chars.  Then identify the method and resource from the
            // split values.
            var split = line.Split(' ');
            var httpMethod = HeaderBase.GetHttpMethod(split[0].ToLower());
            var resource = split[1];

            return new Tuple<HttpMethod, string>(httpMethod, resource);
        }
    }
}