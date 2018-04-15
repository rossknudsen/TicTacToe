using System;
using System.IO;

namespace TicTacToe
{
    /// <summary>
    /// This class is used to keep files in sync between two directories.
    /// </summary>
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
