using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    /// <summary>
    /// This class forms the base for the HTTP request and response
    /// </summary>
    public abstract class MessageBase
    {
        private const string ContentLengthHeaderKey = "content-length";

        protected MessageBase(Dictionary<string, string> headers, byte[] body = null)
        {
            Headers = headers;
            Body = body;
            Headers["Connection"] = "close";
        }

        /// <summary>
        /// A dictionary containing the headers of the request or response
        /// </summary>
        public Dictionary<string, string> Headers { get; }

        /// <summary>
        /// The body of the request or response (if any).  Is null if there is no body.
        /// </summary>
        public byte[] Body { get; }

        /// <summary>
        /// This method converts this message object to binary form suitable for
        /// transmission over a network.
        /// </summary>
        /// <returns>An array of bytes containing this message in binary format.</returns>
        public byte[] ToBytes()
        {
            // check if the body exists.
            if (Body != null && Body.Length > 0)
            {
                // add/update the content length header based on the length of the body.
                Headers[ContentLengthHeaderKey] = Body.Length.ToString();
            }
            else
            {
                // remove the content length header if there is no body.
                Headers.Remove(ContentLengthHeaderKey);
            }

            // get the body data or just an empty array.
            var bodyData = Body ?? new byte[0];
            
            // create the first line of the message and then add the following header lines.
            var responseBuilder = new StringBuilder();
            responseBuilder.Append(GenerateFirstLine());
            foreach (var header in Headers)
            {
                responseBuilder.Append($"{header.Key}: {header.Value}\n");
            }

            // add an extra blank line to indicate the end of the header.
            responseBuilder.Append("\n");

            // convert the header to bytes and concatenate the body data.
            return Encoding.UTF8.GetBytes(responseBuilder.ToString())
                .Concat(bodyData)
                .ToArray();
        }

        /// <summary>
        /// This method must be implemented in derived classes to provide a way to create the
        /// first line of the message to be used in converting the message to binary format.
        /// </summary>
        /// <returns>A string representing the first line of this message when converted to bytes.</returns>
        protected abstract string GenerateFirstLine();
    }
}