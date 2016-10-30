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
            var localTree = localFSManager.GetTree(@"C:\GDrive", 2);

            var googleFSManager = new GoogleDriveFileSystemLib.FileManager(container.Resolve<IGoogleDriveService>());

            var root = googleFSManager.GetTree("root", 2);
            
            /*foreach (var child in root.Children)
            {
                Console.Write(child.Name);
                if (localTree.Children.Any(x => x.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.Write(" V");
                    Console.WriteLine("");
                }
                Console.WriteLine("");
            }*/

            var comparisonResult = Utils.CompareTwoEnumerables(root.Children, localTree.Children);

            var initialCursorTop = Console.CursorTop;
            var initialCursorLeft = Console.CursorLeft;

            foreach (var driveChild in root.Children.OrderBy(x => x.NodeType).ThenBy(x => x.Name))
            {
                Console.ForegroundColor = comparisonResult.CommonNodesFistSequenceIds.Contains(driveChild.Id)
                    ? ConsoleColor.Green
                    : ConsoleColor.White;
                Console.WriteLine(driveChild.Name);
            }

            Console.CursorTop = initialCursorTop;
            Console.CursorLeft = 50;

            foreach (var localChild in localTree.Children.OrderBy(x => x.NodeType).ThenBy(x => x.Name))
            {
                Console.CursorLeft = 50;
                Console.ForegroundColor = comparisonResult.CommonNodesSecondSequenceIds.Contains(localChild.Id)
                    ? ConsoleColor.Green
                    : ConsoleColor.White;
                Console.WriteLine(localChild.Name);
            }

            Console.CursorLeft = initialCursorLeft;

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