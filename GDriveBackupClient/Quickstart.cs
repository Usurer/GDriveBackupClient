using System;
using System.Text;
using Autofac;
using GDriveClientLib.Abstractions;
using Worker;
using Worker.Cache;
using GoogleFileManager = GoogleDriveFileSystemLib.FileManager;
using LocalFileManager = LocalFileSystemLib.FileManager;

namespace GDriveBackupClient
{
    // TODO: Integrate with Trello!
    internal class DriveCommandLineSample
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            try
            {
                Console.WriteLine("========== START ==========");

                var container = new Initializer().RegisterComponents();

                var localManager = new LocalFileManager();
                var googleManager = new GoogleFileManager(container.Resolve<IGoogleDriveService>());
                // TODO: both cache managers should be singletons. I'm a bit worried about possible issues with race conditions when reading/writing, but I don't think this is something serious here
                var fileCacheManager = new FileCacheManager("filecache.txt");
                var memoryCacheManager = new MemoryCacheManager();

                var app = new Application(googleManager, localManager, fileCacheManager, memoryCacheManager);
                app.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}