using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Requests
{
    public class Request
    {
        private Request(HttpMethod method, string requestedResource, Dictionary<string, string> headers, string body)
        {
            Method = method;
            Resource = requestedResource;
            Headers = headers;
            Body = body;
        }

        public HttpMethod Method { get; }

        public string Resource { get; }

        public Dictionary<string, string> Headers { get; }

        public string Body { get; }

        public static bool TryParseRequest(byte[] data, out Request request)
        {
            var requestString = Encoding.UTF8.GetString(data);

            var lines = requestString
                .Replace("\r", "")
                .Split(new []{ '\n' }, StringSplitOptions.None)
                .ToList();

            var headers = new Dictionary<string, string>();
            var bodyBuilder = new StringBuilder();

            using (var enumerator = lines.GetEnumerator())
            {
                enumerator.MoveNext();

                var requestedResource = ReadFirstLine(enumerator.Current);

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

                request = new Request(requestedResource.method, requestedResource.resource, headers, bodyBuilder.ToString());
                return true;
            }
        }

        private static void ReadHeaderLine(string line, Dictionary<string, string> headers)
        {
            var split = line.Split(':');
            var key = split[0].Trim(' ');
            var value = split[1].Trim(' ');
            headers[key] = value;
        }

        private static (HttpMethod method, string resource) ReadFirstLine(string line)
        {
            var split = line.Split(' ');
            var httpMethod = GetHttpMethod(split[0].ToLower());
            var resource = split[1];

            return (httpMethod, resource);
        }

        private static HttpMethod GetHttpMethod(string httpVerb)
        {
            switch (httpVerb.ToLower())
            {
                case "get":
                    return HttpMethod.Get;

                case "post":
                    return HttpMethod.Post;

                default:
                    throw new ArgumentOutOfRangeException(nameof(httpVerb));
            }
        }
    }
}