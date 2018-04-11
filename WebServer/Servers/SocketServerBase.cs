using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Responses;

namespace WebServer.Servers
{
    public abstract class SocketServerBase
    {
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
                socket.Listen(10); // TODO what does this do?

                while (true)
                {
                    using (var handler = socket.Accept())
                    {
                        var data = ReceiveData(handler);
                        if (data.Length > 0)
                        {
                            var response = GenerateResponse(data);
                            handler.Send(response.ToBytes());
                        }

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        handler.Dispose();
                    }
                }
            }
        }

        protected static byte[] ReceiveData(Socket handler)
        {
            const int bufferSize = 1024;
            var bytes = new byte[bufferSize];

            var totalBytesReceived = 0;
            //while (true)
            //{
                totalBytesReceived += handler.Receive(bytes, totalBytesReceived, bufferSize - totalBytesReceived, SocketFlags.None);
            //}

            var result = new byte[totalBytesReceived];
            Array.Copy(bytes, result, totalBytesReceived);
            return result;
        }

        protected abstract Response GenerateResponse(byte[] data);
    }
}
