using System;

namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO get the following parameters from config.
            // the configuration for the webserver.
            var host = "127.0.0.1";
            var webServerPort = 8080;
            var gameServerPort = 8081;
            var sourcePath = @"..\..\wwwroot\"; // TODO find a better way to reference this directory.
            var outputPath = @".\wwwroot\";

            // create a web server instance and run it.
            var webserver = new WebServer(host, webServerPort, host, gameServerPort);
            webserver.Run();

            // create a new file sync object to aid with development.  It keeps changes to the web
            // content syncronised with what the running web server has in the output directory.
            using (var sync = new HtmlFileSync(sourcePath, outputPath))
            {
                Console.WriteLine("Press any key to exit application...");
                Console.ReadKey();
            }
        }
    }
}
