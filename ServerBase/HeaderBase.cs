using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public abstract class HeaderBase : Dictionary<string, string>
    {
        protected static List<string> ConvertDataToLines(byte[] headerData)
        {
            var requestString = Encoding.ASCII.GetString(headerData);

            var lines = requestString
                .Replace("\r", "")
                .Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            return lines;
        }

        protected static (string key, string value) ReadHeaderLine(string line)
        {
            var split = line.Split(':');
            var key = split[0].Trim(' ');
            var value = split[1].Trim(' ');
            return (key, value);
        }

        protected static Dictionary<string, string> ParseHeaders(IEnumerable<string> lines)
        {
            return lines.Select(ReadHeaderLine)
                .ToDictionary(p => p.key, p => p.value);
        }

        protected static HttpMethod GetHttpMethod(string httpVerb)
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