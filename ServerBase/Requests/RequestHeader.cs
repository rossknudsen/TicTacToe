using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Requests
{
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

        public HttpMethod Method { get; }

        public string Resource { get; }

        public static bool TryParseHeader(byte[] headerData, out RequestHeader header)
        {
            var lines = ConvertDataToLines(headerData);
            var headers = ParseHeaders(lines.Skip(1));
            var firstLine = ReadFirstLine(lines.First());

            header = new RequestHeader(firstLine.method, firstLine.resource, headers);
            return true;
        }

        private static (HttpMethod method, string resource) ReadFirstLine(string line)
        {
            var split = line.Split(' ');
            var httpMethod = HeaderBase.GetHttpMethod(split[0].ToLower());
            var resource = split[1];

            return (httpMethod, resource);
        }
    }
}