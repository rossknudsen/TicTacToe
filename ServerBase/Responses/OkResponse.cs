using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicTacToe.Responses
{
    /// <summary>
    /// Represents an Ok HTTP response.
    /// </summary>
    public class OkResponse : Response
    {
        /// <summary>
        /// A mappng between file extensions and the content type.
        /// </summary>
        private static readonly Dictionary<string, string> FileExtensionToContentTypeMap = new Dictionary<string, string>
        {
            { ".html", "text/html" },
            { ".jpg", "image/jpeg" },
            { ".js", "text/javascript" },
            { ".css", "text/css" }
        };

        public OkResponse(string responseBody) : this(Encoding.UTF8.GetBytes(responseBody))
        {
            Headers["Content-Type"] = "application/json";
        }

        public OkResponse(FileInfo fileInfo) : this(File.ReadAllBytes(fileInfo.FullName))
        {
            // use the type mapping dictionary to provide the content type header.
            Headers["Content-Type"] = FileExtensionToContentTypeMap[fileInfo.Extension];
        }

        public OkResponse(byte[] responseBytes)
            : base(HttpResponseCode.Ok, "Ok", new Dictionary<string, string>(), responseBytes)
        {
            // in all cases we tell the browser not to cache the result and notify about the body length.
            Headers["Content-Length"] = responseBytes.Length.ToString();
            Headers["Cache-Control"] = "no-cache";
        }
    }
}