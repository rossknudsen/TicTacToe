using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WebServer
{
    public class Request
    {
        private const string RequestRegexPattern = @"GET (\S*) .*";

        public Request(string requestedResource)
        {
            Resource = requestedResource;
        }

        public string Resource { get; }

        public static bool TryParseRequest(byte[] data, out Request request)
        {
            var requestString = Encoding.ASCII.GetString(data);

            var match = Regex.Match(requestString, RequestRegexPattern, RegexOptions.Multiline);
            if (!match.Success)
            {
                request = null;  // TODO should return a BadRequest response here.
                return false;
            }

            var requestedResource = match.Groups[1].Value;

            request = new Request(requestedResource);
            return true;
        }
    }
}