using System;
using System.IO;

namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO get the following parameters from config.
            var host = "127.0.0.1";
            var webServerPort = 8080;
            var gameServerPort = 8081;
            var sourcePath = @"..\..\wwwroot\"; // TODO find a better way to reference this directory.
            var outputPath = @".\wwwroot\";

            var webserver = new Servers.WebServer(host, webServerPort, host, gameServerPort);
            webserver.Run();

            using (var sync = new HtmlFileSync(sourcePath, outputPath))
            {
                Console.WriteLine("Press any key to exit application...");
                Console.ReadKey();
            }
        }
    }

    internal class HtmlFileSync : IDisposable
    {
        private readonly string _outputPath;
        private readonly FileSystemWatcher _watcher;

        public HtmlFileSync(string watchPath, string outputPath)
        {
            _outputPath = outputPath;
            _watcher = new FileSystemWatcher(watchPath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = 
                    NotifyFilters.LastAccess 
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName 
                    | NotifyFilters.DirectoryName
            };
            _watcher.Changed += WatcherOnChanged;
            _watcher.Created += WatcherOnChanged;
            _watcher.Deleted += WatcherOnChanged;
            _watcher.Renamed += WatcherOnChanged;
            _watcher.EnableRaisingEvents = true;

            // TODO perform an initial population of the directory.
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var fileName = Path.GetFileName(fileSystemEventArgs.Name);
            var outputPath = Path.Combine(_outputPath, fileName);

            try
            {
                switch (fileSystemEventArgs.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        File.Copy(fileSystemEventArgs.FullPath, outputPath);
                        break;

                    case WatcherChangeTypes.Deleted:
                        File.Delete(outputPath);
                        break;

                    case WatcherChangeTypes.Changed:
                        File.Copy(fileSystemEventArgs.FullPath, outputPath, true);
                        break;

                    case WatcherChangeTypes.Renamed:
                        File.Move(fileSystemEventArgs.FullPath, outputPath);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}
