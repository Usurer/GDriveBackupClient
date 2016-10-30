using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using GDriveClientLib.Abstractions;

using LocalFileSystemLib;
using GoogleDriveFileSystemLib;

namespace GDriveBackupClient
{
    internal class DriveCommandLineSample
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("========== START ==========");
            
            IContainer container = new Initializer().RegisterComponents();

            var localFSManager = new LocalFileSystemLib.FileManager();
            var googleFSManager = new GoogleDriveFileSystemLib.FileManager(container.Resolve<IGoogleDriveService>());

            var root = googleFSManager.GetTree("root", 2);
            foreach (var child in root.Children)
            {
                Console.WriteLine(child.Name);
            }
            
            /*var backups =
                googleFSManager.GetTree("root", 1)
                    .Children.Where(x => x.Name.Equals("backups", StringComparison.InvariantCultureIgnoreCase))
                    .Select((x, index) => new KeyValuePair<int, INode>(index, x))
                    .ToArray();

            foreach (var backup in backups)
            {
                Console.WriteLine("{0}. {1}", backup.Key, backup.Value.Id);
            }

            var first = backups.First();

            var localFiles = localFSManager.GetTree(@"G:\DATA", null);
            ListAbsent(localFiles, first.Value);*/

            Console.WriteLine("=============");
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void ListAbsent(INode localNode, INode remoteNode)
        {
            if(remoteNode == null || localNode == null) return;

            Console.WriteLine("Absent for local node {0} in remote node {1}", localNode.Name, remoteNode.Name);

            foreach (var file in localNode.Children)
            {
                var remoteFile = remoteNode.Children.SingleOrDefault(x => x.Name == file.Name);
                if (remoteFile == null)
                {
                    Console.WriteLine(file.Name);
                    File.AppendAllText(@"log.txt", file.Name);
                    continue;
                }
                
                ListAbsent(file, remoteFile);
            }
        }
    }
}