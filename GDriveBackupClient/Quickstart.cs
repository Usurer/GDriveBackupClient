using System;
using System.Text;
using Autofac;
using GDriveClientLib.Interfaces;

namespace GDriveBackupClient
{
    internal class DriveCommandLineSample
    {
        private static void Main(string[] args)
        {
            IContainer container = new Initializer().RegisterComponents();

            Console.OutputEncoding = Encoding.Unicode;

            /*using (var stream = File.OpenRead("document.txt"))
            {
                var uploader = container.Resolve<IUploader>();
                uploader.Upload(service, stream);    
            }*/

            var reader = container.Resolve<IReader>();
            reader.DrawRootTree();
            Console.WriteLine("=============");
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}