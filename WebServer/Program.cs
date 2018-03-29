using System.Net;
using WebServer.Servers;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO get the following parameters from config.
            var host = "127.0.0.1";
            var webServerPort = 8080;
            var gameServerPort = 8081;

            var webserver = new Servers.WebServer(host, webServerPort);
            webserver.Run();

            var gameServer = new GameServer(host, gameServerPort);
            gameServer.Run();

            while (true)
            {
                // do nothing
            }
        }
    }
}
