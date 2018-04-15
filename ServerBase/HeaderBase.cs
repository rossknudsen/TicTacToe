using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    /// <summary>
    /// The class forms the base class for the HTTP request and response headers
    /// </summary>
    public abstract class HeaderBase : Dictionary<string, string>
    {
        /// <summary>
        /// Transforms the provided header data into a list of strings representing the lines
        /// in the header message.
        /// </summary>
        /// <param name="headerData">Header information in binary form.</param>
        /// <returns>A list of strings representing the lines in the header information.</returns>
        protected static List<string> ConvertDataToLines(byte[] headerData)
        {
            // convert the binary data into a string.
            var requestString = Encoding.ASCII.GetString(headerData);

            // split the string by removing carriage returns and splitting on line ending chars.
            var lines = requestString
                .Replace("\r", "")
                .Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            return lines;
        }

        /// <summary>
        /// This method is used to parse the header lines excluding the first line.
        /// </summary>
        /// <param name="line">A string representing a line in the header.</param>
        /// <returns>A tuple containing the key and value from the header line.</returns>
        protected static Tuple<string, string> ReadHeaderLine(string line)
        {
            var split = line.Split(':');
            var key = split[0].Trim(' ');
            var value = split[1].Trim(' ');
            return new Tuple<string, string>(key, value);
        }

        /// <summary>
        /// This method takes all the header lines, parses them and then returns the result in
        /// a dictionary of key value pairs.
        /// </summary>
        /// <param name="lines">The header lines.</param>
        protected static Dictionary<string, string> ParseHeaders(IEnumerable<string> lines)
        {
            return lines.Select(ReadHeaderLine)
                .ToDictionary(p => p.Item1, p => p.Item2);
        }

        /// <summary>
        /// Converts a HTTP verb in string form to a <see cref="HttpMethod"/> enumeration value.
        /// </summary>
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