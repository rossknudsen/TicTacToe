﻿using System;
using System.Net;
using System.Net.Sockets;
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
                        var response = GenerateResponse(data);
                        SendResponse(handler, response);
                    }
                }
            }
        }

        private byte[] ReceiveData(Socket handler)
        {
            // TODO handle requests that are longer than 1024 bytes long.
            var bytes = new byte[1024];
            var bytesCount = handler.Receive(bytes);
            var result = new byte[bytesCount];
            Array.Copy(bytes, result, bytesCount);
            return result;
        }

        protected abstract Response GenerateResponse(byte[] data);

        private void SendResponse(Socket handler, Response response)
        {
            handler.Send(response.ToBytes());
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}