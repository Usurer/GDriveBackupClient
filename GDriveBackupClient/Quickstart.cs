using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using LocalFileSystemLib;
using GoogleDriveFileSystemLib;
using GoogleFileManager = GoogleDriveFileSystemLib.FileManager;
using LocalFileManager = LocalFileSystemLib.FileManager;


namespace GDriveBackupClient
{
    internal class DriveCommandLineSample
    {
        public static void Main(string[] args)
        {
            try
            {
                StartApplication();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
        }

        private static void StartApplication()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("========== START ==========");

            IContainer container = new Initializer().RegisterComponents();

            var localFSManager = new LocalFileManager();
            var googleFSManager = new GoogleFileManager(container.Resolve<IGoogleDriveService>());

            var backupsFolder = LoadGoogleBackupsFolder(googleFSManager);

            Console.WriteLine("Enumerating {0} Backups folder children", backupsFolder.Children.Count());
            var children = backupsFolder.Children.Select(child => googleFSManager.GetTree(child.Id)).ToList();

            Console.WriteLine("Getting local data for {0}", @"G:\DATA");
            var localRoot = localFSManager.GetTree(@"G:\DATA");

            Console.WriteLine("Enumerating {0} local FS children", localRoot.Children.Count());
            foreach (var child in localRoot.Children)
            {
                var childData = localFSManager.GetTree(child.Id);
                child.Name = childData.Name;
                child.NodeType = childData.NodeType;
                child.Children = childData.Children;

                if (
                    !children.Where(g => !string.IsNullOrEmpty(g.Name))
                        .Any(g => g.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine(" ! {0}", child.Name);
                }
                if (
                    children.Where(g => !string.IsNullOrEmpty(g.Name))
                        .Any(g => g.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Console.WriteLine(" + {0}", child.Name);
                }
            }

            Console.WriteLine("=============");
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static INode LoadGoogleBackupsFolder(IFileManager googleFSManager)
        {
            var loadedId = ReadBackupsNodeId();
            if (string.IsNullOrEmpty(loadedId))
            {
                Console.WriteLine("No saved data about Backups folder Id");
                return FindGoogleBackupsFolder(googleFSManager);
            }
            Console.WriteLine("Backups folder Id loaded. Getting data from Google");
            return googleFSManager.GetTree(loadedId);
        }

        private static INode FindGoogleBackupsFolder(IFileManager googleFSManager)
        {
            Console.WriteLine("Getting google data for {0}", "root");
            var googleRoot = googleFSManager.GetTree("root");

            INode backupsFolder = null;

            Console.WriteLine("Enumerating {0} google children, looking for Backups folder", googleRoot.Children.Count());
            foreach (var child in googleRoot.Children)
            {
                Console.Write(".");
                var childData = googleFSManager.GetTree(child.Id);
                if (childData.Name == "Backups")
                {
                    Console.WriteLine("Backups folder found");
                    backupsFolder = childData;
                    break;
                }
            }

            if (backupsFolder == null)
            {
                throw new ApplicationException("No folder named 'Backups' found on Google Drive");
            }

            StoreBackupsNodeId(backupsFolder);
            return backupsFolder;
        }

        private static void StoreBackupsNodeId(INode backupsNode)
        {
            File.WriteAllText("backupsFolderId.txt", backupsNode.Id);
            Console.WriteLine("Backups node Id stored");
        }

        private static string ReadBackupsNodeId()
        {
            if (File.Exists("backupsFolderId.txt"))
            {
                Console.WriteLine("Backups node Id loaded");
                return File.ReadAllLines("backupsFolderId.txt").FirstOrDefault();
            }
            return string.Empty;
        }
    }
}