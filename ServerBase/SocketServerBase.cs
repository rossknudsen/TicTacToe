using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TicTacToe.Requests;
using TicTacToe.Responses;

namespace TicTacToe
{
    /// <summary>
    /// This class represents the base class for the web and game servers.
    /// </summary>
    public abstract class SocketServerBase
    {
        private static readonly byte CarriageReturn = System.Text.Encoding.ASCII.GetBytes("\r")[0];
        private static readonly byte LineEnd = System.Text.Encoding.ASCII.GetBytes("\n")[0];

        private readonly IPEndPoint _endpoint;

        protected SocketServerBase(string host, int port)
        {
            // set up the endpoint using the provided host and port.
            _endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        /// <summary>
        /// This method is called by external client code and will run the server using the provided
        /// parameters.
        /// </summary>
        public void Run()
        {
            // run the server code on a separate thread.
            //var task = Task.Run(RunServer);
            RunServer();
        }

        /// <summary>
        /// This is the internal implementation of the server process.
        /// </summary>
        /// <returns></returns>
        private Task RunServer()
        {
            // create a socket to listen on the endpoint.
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // bind the socket and start listening.
                socket.Bind(_endpoint);
                socket.Listen(10);

                // begin an infinite loop to listen for connection attempts.
                while (true)
                {
                    // when an connection is made, create a separate socket to handle the request/response.
                    using (var handler = socket.Accept())
                    {
                        handler.ReceiveTimeout = 5000;

                        // read the header data from the client.
                        var headerData = ReceiveHeaderData(handler);

                        // if we actually got data, then create a response and send it back to the cliet.
                        if (headerData != null)
                        {
                            var response = DetermineResponse(handler, headerData);
                            handler.Send(response.ToBytes());
                        }

                        // close the connection to indicate the end of the response.
                        CloseSocket(handler);
                    }
                }
            }
        }

        /// <summary>
        /// This method creates a response from the provided header data.
        /// </summary>
        /// <param name="handler">The socket that is handling the current HTTP request</param>
        /// <param name="headerData">The header data that has been read from the client.</param>
        /// <returns>A HTTP response to the client's request.</returns>
        private Response DetermineResponse(Socket handler, byte[] headerData)
        {
            // Attempt to parse the header from the read header data.
            Response response;
            if (!RequestHeader.TryParseHeader(headerData, out var header))
            {
                // if we can't make sense of the header, return a bad request response.
                response = new BadRequestResponse();
            }
            else if (header.Method == HttpMethod.Post)  // otherwise check if there should be a body in the request.
            {
                // there is a body, check the length header value.
                if (!header.ContainsKey("content-length"))
                {
                    // if the client has not provided the length of the body we request that it is provided.  I
                    // found it difficult to know how long the body is, if it is not provided.
                    response = new LengthRequiredResponse();
                }
                else
                {
                    // get the length of the body and create the request object from the header and body.
                    var bodyLength = Convert.ToInt32(header["content-length"]);
                    var bodyData = ReceiveBodyData(handler, bodyLength);
                    var request = new Request(header, bodyData);
                    
                    // obtain a response to the request from the derived implementation of GenerateResponse.
                    response = GenerateResponse(request);
                }
            }
            else
            {
                // create a request object with no body and generate a response to that request.
                var request = new Request(header);
                response = GenerateResponse(request);
            }

            // return the response object.
            return response;
        }

        /// <summary>
        /// This method closes and disposes of the provided socket.
        /// </summary>
        /// <param name="handler">The socket to clean up.</param>
        private static void CloseSocket(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close(5);  // allow any data that is pending to be sent for 5 seconds
            handler.Dispose();
        }

        /// <summary>
        /// This method recieves the header data from a connected client.
        /// </summary>
        /// <param name="handler">The <see cref="Socket"/> that the client has connected on.</param>
        /// <returns></returns>
        protected byte[] ReceiveHeaderData(Socket handler)
        {
            // we keep track of the number of line ending characters we have seen recently.
            var lineEndingCount = 0;

            // we will keep the data we read from the client in a memory stream.
            var headerStream = new MemoryStream();

            while (true)
            {
                // retrieve a byte from the client.
                var data = new byte[1];
                int bytesCount;
                try
                {
                    bytesCount = handler.Receive(data, 0, 1, SocketFlags.None);
                }
                catch (SocketException)
                {
                    return null;
                }
                var datum = data[0];

                // if the number of bytes read is one then we have data.
                if (bytesCount == 1)
                {
                    headerStream.WriteByte(datum);

                    // we count the number of contiguous line ending chars ignoring carriage returns.
                    if (datum == LineEnd)
                    {
                        lineEndingCount++;
                    }
                    else if (datum != CarriageReturn)
                    {
                        // reset line ending count.
                        lineEndingCount = 0;
                    }

                    // if we get two line ending chars in a row then we have finished reading the header.
                    if (lineEndingCount == 2)
                    {
                        // copy the data to a new array and return it.
                        var result = new byte[headerStream.Length];
                        Array.Copy(headerStream.GetBuffer(), result, headerStream.Length);
                        return result;
                    }
                }
                else if (bytesCount == 0)
                {
                    // if we don't read any data, then we just return null.  Not sure why this happens sometimes.
                    return null;
                }
            }
        }

        /// <summary>
        /// This method will read the specified number of byts from the provided <see cref="Socket"/> and return it.
        /// </summary>
        /// <param name="handler">The <see cref="Socket"/> to be read from.</param>
        /// <param name="bodyLength">The number of bytes to read.</param>
        /// <returns>The read data as a byte array.</returns>
        protected byte[] ReceiveBodyData(Socket handler, int bodyLength)
        {
            // we know how many bytes to expect, so we create an appropriate array and keep track 
            // of how many bytes we've received.
            var result = new byte[bodyLength];
            var totalBytesReceived = 0;

            while (true)
            {
                // try to read the remaining number of bytes we were requested to read.
                totalBytesReceived += handler.Receive(result, totalBytesReceived, bodyLength - totalBytesReceived, SocketFlags.None);

                // check if we've read all the data we need to.
                if (totalBytesReceived >= bodyLength)
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// This method is implemented by derived types, and creates a HTTP response to the provided HTTP request.
        /// </summary>
        /// <param name="request">The request that requires a response.</param>
        /// <returns>The response to the request.</returns>
        protected abstract Response GenerateResponse(Request request);
    }
}
