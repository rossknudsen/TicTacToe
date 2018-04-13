using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TicTacToe.Requests;
using TicTacToe.Responses;

namespace TicTacToe
{
    public abstract class SocketServerBase
    {
        private static readonly byte CarriageReturn = System.Text.Encoding.ASCII.GetBytes("\r")[0];
        private static readonly byte LineEnd = System.Text.Encoding.ASCII.GetBytes("\n")[0];

        private readonly IPEndPoint _endpoint;

        protected SocketServerBase(string host, int port)
        {
            _endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public void Run()
        {
            var task = Task.Run(RunServer);
        }

        private Task RunServer()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(_endpoint);
                socket.Listen(10);

                while (true)
                {
                    using (var handler = socket.Accept())
                    {
                        var headerData = ReceiveHeaderData(handler);

                        if (headerData != null)
                        {
                            var response = DetermineResponse(handler, headerData);
                            handler.Send(response.ToBytes());
                        }

                        CloseSocket(handler);
                    }
                }
            }
        }

        private Response DetermineResponse(Socket handler, byte[] headerData)
        {
            Response response;
            if (!RequestHeader.TryParseHeader(headerData, out var header))
            {
                response = new BadRequestResponse();
            }
            else if (header.Method == HttpMethod.Post)
            {
                // there is a body, check the length header value.
                if (!header.ContainsKey("content-length"))
                {
                    response = new LengthRequiredResponse();
                }
                else
                {
                    var bodyLength = Convert.ToInt32(header["content-length"]);
                    var bodyData = ReceiveBodyData(handler, bodyLength);
                    var request = new Request(header, bodyData);
                    response = GenerateResponse(request);
                }
            }
            else
            {
                var request = new Request(header);
                response = GenerateResponse(request);
            }

            return response;
        }

        private static void CloseSocket(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close(5);  // allow any data that is pending to be sent for 5 seconds
            handler.Dispose();
        }

        protected byte[] ReceiveHeaderData(Socket handler)
        {
            var lineEndingCount = 0;
            var headerStream = new MemoryStream();

            while (true)
            {
                var data = new byte[1];
                var bytesCount = handler.Receive(data, 0, 1, SocketFlags.None);
                var datum = data[0];
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
                        var result = new byte[headerStream.Length];
                        Array.Copy(headerStream.GetBuffer(), result, headerStream.Length);
                        return result;
                    }
                }
                else if (bytesCount == 0)
                {
                    return null;
                }
            }
        }

        protected byte[] ReceiveBodyData(Socket handler, int bodyLength)
        {
            // we know how many bytes to expect, so we create an appropriate array and keep track 
            // of how many bytes we've received.
            var result = new byte[bodyLength];
            var totalBytesReceived = 0;

            while (true)
            {
                totalBytesReceived += handler.Receive(result, totalBytesReceived, bodyLength - totalBytesReceived, SocketFlags.None);

                if (totalBytesReceived >= bodyLength)
                {
                    return result;
                }
            }
        }

        protected abstract Response GenerateResponse(Request request);
    }
}
