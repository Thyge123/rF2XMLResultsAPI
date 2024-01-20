using Newtonsoft.Json;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;

namespace rF2XMLTestAPI.Model
{
    public class FileWatcherService : IHostedService, IDisposable
    {
        private FileSystemWatcher _fileWatcher;
        private rFactorXMLManager _manager;
        private readonly IServiceProvider _serviceProvider;

        public FileWatcherService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void WaitForFile(string fullPath)
        {
            while (true)
            {
                try
                {
                    using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(1000); // Wait and retry
                }
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
            _fileWatcher.Filter = "*.xml";
            _fileWatcher.Created += FileCreatedHandler;
            _fileWatcher.EnableRaisingEvents = true; // Start monitoring the directory

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _fileWatcher.EnableRaisingEvents = false; // Stop monitoring the directory
            _fileWatcher.Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
        }


        private void FileCreatedHandler(object sender, FileSystemEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                if (e.ChangeType == WatcherChangeTypes.Created && e.FullPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var driverContext = scope.ServiceProvider.GetRequiredService<DriverContext>();
                    var lapsContext = scope.ServiceProvider.GetRequiredService<LapsContext>();
                    var raceResultContext = scope.ServiceProvider.GetRequiredService<RaceResultContext>();
                    _manager = new rFactorXMLManager(driverContext, lapsContext, raceResultContext);
                    WaitForFile(e.FullPath);
                    _manager.LoadLatestResult();
                    Console.WriteLine($"New XML file created: {e.FullPath}");
                }
            }
        } 
    }
}