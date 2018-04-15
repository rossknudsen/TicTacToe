using System;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO get the following parameters from config.
            var host = "127.0.0.1";
            var gameServerPort = 8081;

            // create a game server and run it.
            var gameServer = new GameServer(host, gameServerPort);
            gameServer.Run();

            Console.WriteLine("Press any key to exit application...");
            Console.ReadKey();
        }
    }
}
