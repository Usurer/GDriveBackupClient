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
using Newtonsoft.Json;
using GoogleFileManager = GoogleDriveFileSystemLib.FileManager;
using LocalFileManager = LocalFileSystemLib.FileManager;


namespace GDriveBackupClient
{
    // TODO: Integrate with Trello!
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
            /*TODO: I want these Console calls to be done via some ILogger interface. In this class it's useless, but since I don't want
            Console to be used in other namespaces - I want them to receive the ILogger and just write stuff to Debug, Error etc log.
            So for Console app I'd be able to use simple class that would write to Console as an ILogger implementation.*/
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("========== START ==========");

            IContainer container = new Initializer().RegisterComponents();

            var localFSManager = new LocalFileManager();
            var googleFSManager = new GoogleFileManager(container.Resolve<IGoogleDriveService>());

            var backupsFolder = LoadGoogleBackupsFolder(googleFSManager);
            PrintTree(backupsFolder, googleFSManager, localFSManager);

            var foldersStack = new Stack<INode>();
            foldersStack.Push(backupsFolder);

            while (true)
            {
                Console.WriteLine("Enter a name of a folder to open or press enter to go up");
                var folderToOpen = Console.ReadLine();
                if (string.IsNullOrEmpty(folderToOpen))
                {
                    if (foldersStack.Count == 0)
                    {
                        Console.WriteLine("Oups, that was an upper level");
                        break;
                    }

                    // Throwing latest folder away
                    foldersStack.Pop();
                }
                else
                {
                    Console.WriteLine($"Looking for {folderToOpen} in {foldersStack.Peek().Name}");
                    var folder = LoadSelectedFolderFromGoogle(googleFSManager, foldersStack.Peek().Id, folderToOpen);
                    if (folder == null)
                    {
                        Console.WriteLine("Folder not found");
                        continue;
                    }

                    foldersStack.Push(folder);
                }

                PrintTree(foldersStack.Peek(), googleFSManager, localFSManager);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void PrintTree(INode backupsFolder, GoogleFileManager googleFSManager, LocalFileManager localFSManager)
        {
            Console.WriteLine($"Enumerating {backupsFolder.Children.Count()} {backupsFolder.Name} children");
            var children = backupsFolder.Children.Select(child => googleFSManager.GetTree(child.Id)).ToList();

            /*Console.WriteLine("Getting local data for {0}", @"G:\DATA");
            var localRoot = localFSManager.GetTree(@"G:\DATA");*/
            foreach (var child in children)
            {
                Console.WriteLine(child.Name);
            }

            Console.WriteLine("=============");
        }

        private static INode LoadGoogleBackupsFolder(IFileManager googleFSManager, string currentRootPath = "root", string folderName = "Backups")
        {
            Console.WriteLine("Searching for first AKA root level stuff");
            var currentRoot = googleFSManager.GetTree(currentRootPath);
            var loadedId = ReadBackupsNodeId(currentRoot, folderName);

            if (string.IsNullOrEmpty(loadedId))
            {
                Console.WriteLine("No saved data about Backups folder Id");
                return FindGoogleBackupsFolder(googleFSManager, currentRoot, folderName);
            }
            Console.WriteLine("Backups folder Id loaded. Getting data from Google");
            return googleFSManager.GetTree(loadedId);
        }

        private static INode LoadSelectedFolderFromGoogle(IFileManager googleFSManager, string currentRootId, string folderName)
        {
            var currentRoot = googleFSManager.GetTree(currentRootId);
            var folder = FindGoogleBackupsFolder(googleFSManager, currentRoot, folderName);
            return googleFSManager.GetTree(folder.Id);
        }

        private static INode FindGoogleBackupsFolder(IFileManager googleFSManager, INode currentRoot, string folderName)
        {
            Console.WriteLine($"Getting google data for '{currentRoot.Name}'");

            INode backupsFolder = null;

            Console.WriteLine($"Enumerating {currentRoot.Children.Count()} google children, looking for {folderName} folder");
            foreach (var child in currentRoot.Children)
            {
                Console.Write(".");
                var childData = googleFSManager.GetTree(child.Id);
                if (childData.Name == folderName)
                {
                    Console.WriteLine($"{folderName} folder found");
                    backupsFolder = childData;
                    break;
                }
            }

            if (backupsFolder == null)
            {
                throw new ApplicationException($"No folder named '{folderName}' found in Google Drive {currentRoot.Name}");
            }

            if (folderName == "Backups")
            {
                StoreBackupsNodeId(backupsFolder, currentRoot);
            }

            return backupsFolder;
        }

        // TODO: Multiple nodes to be stored
        private static void StoreBackupsNodeId(INode backupsNode, INode rootNode)
        {
            File.WriteAllText("backupsFolderId.txt", JsonConvert.SerializeObject(new NodeBackupData(backupsNode, rootNode)));
            Console.WriteLine($"{backupsNode.Name} node Id stored");
        }

        // TODO: Search root by ID.
        private static string ReadBackupsNodeId(INode currentRoot, string nodeName)
        {
            if (File.Exists("backupsFolderId.txt"))
            {
                Console.WriteLine("Looking in backups file");
                var savedData = JsonConvert.DeserializeObject<NodeBackupData>(File.ReadAllLines("backupsFolderId.txt").FirstOrDefault());
                if (savedData.RootId == currentRoot.Id && savedData.NodeName == nodeName)
                {
                    Console.WriteLine("Saved dara found");
                    return savedData.NodeId;
                }
            }
            return string.Empty;
        }

        private class NodeBackupData
        {
            public NodeBackupData()
            {
            }

            public NodeBackupData(INode backupNode, INode rootNode)
            {
                NodeId = backupNode.Id;
                NodeName = backupNode.Name;
                RootId = rootNode.Id;
                RootName = rootNode.Name;
            }
            public string NodeId { get; set; }

            public string NodeName { get; set; }

            public string RootId { get; set; }

            public string RootName { get; set; }
        }
    }
}