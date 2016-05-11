using System;
using System.Text;
using Autofac;
using GDriveClientLib.Abstractions;
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
                var cacheManager = new FileCacheManager("filecache.txt");

                var app = new Application(googleManager, localManager, cacheManager);
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