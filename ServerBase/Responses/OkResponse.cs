using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicTacToe.Responses
{
    public class OkResponse : Response
    {
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
            Headers["Content-Type"] = FileExtensionToContentTypeMap[fileInfo.Extension];
        }

        public OkResponse(byte[] responseBytes)
            : base(HttpResponseCode.Ok, "Ok", new Dictionary<string, string>(), responseBytes)
        {
            Headers["Content-Length"] = responseBytes.Length.ToString();
            Headers["Cache-Control"] = "no-cache";
        }
    }
}