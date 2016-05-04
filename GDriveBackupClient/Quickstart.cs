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
            var localDataRoot = localFSManager.GetTree(@"E:\CodeLearning\GDriveBackupClient\Data");

            PrintTree(backupsFolder, googleFSManager, localFSManager, localDataRoot);

            /*var googleFolderAncestorsStack = new Stack<INode>();
            googleFolderAncestorsStack.Push(backupsFolder);

            var localFolderAncestorsStack = new Stack<INode>();
            localFolderAncestorsStack.Push(localDataRoot);*/

            var navigator = new TreeNavigator(backupsFolder, localDataRoot, googleFSManager, localFSManager);

            while (true)
            {
                Console.WriteLine("Enter a name of a folder to open or press enter to go up");
                var folderToOpen = Console.ReadLine();
                if (string.IsNullOrEmpty(folderToOpen))
                {
                    navigator.NavigateBothTreesUp();
                }
                else
                {
                    navigator.NavigateBothTreesDown(folderToOpen);
                }

                PrintTree(navigator.GetCurrentGoogleNode(), googleFSManager, localFSManager, navigator.GetCurrentLocalNode());
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void PrintTree(INode backupsFolder, GoogleFileManager googleFSManager, LocalFileManager localFSManager, INode localNode)
        {
            Console.WriteLine($"So we have Google {backupsFolder} and local {localNode}");

            Console.WriteLine($"Enumerating Google's {backupsFolder.Children.Count()} {backupsFolder.Name} children");
            var googleFolderChildren = backupsFolder.Children.Select(child => googleFSManager.GetTree(child.Id)).ToList();

            Console.WriteLine($"Enumerating Local {localNode.Children.Count()} {localNode.Name} at {localNode.Id} children");
            var localFolderChildren = localNode.Children?.Select(child => localFSManager.GetTree(child.Id)).ToList();

            localFolderChildren = localFolderChildren ?? new List<INode>();

            var merge = googleFolderChildren.Concat(localFolderChildren.Where(x => googleFolderChildren.All(y => !y.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))));

            foreach (var child in merge)
            {
                var isInLocal = localFolderChildren.Any(x => x.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase));
                var isInGoogle = googleFolderChildren.Any(x => x.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase));
                var isInBoth = isInLocal && isInGoogle;

                var symbolicPrefix = isInBoth ? "*" : isInLocal ? "L" : "G";

                Console.WriteLine($"{symbolicPrefix} {child.Name} | {child.Id}");
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

        /*private static INode LoadSelectedFolderFromGoogle(IFileManager googleFSManager, string currentRootId, string folderName)
        {
            var currentRoot = googleFSManager.GetTree(currentRootId);
            var folder = FindGoogleBackupsFolder(googleFSManager, currentRoot, folderName);
            return googleFSManager.GetTree(folder.Id);
        }*/

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

        private static void StoreBackupsNodeId(INode nodeToBackup, INode parent)
        {
            var backupRecord = new NodeBackupData(nodeToBackup, parent);
            var storedData = File.Exists("backupsFolderId.txt") ? File.ReadAllLines("backupsFolderId.txt").FirstOrDefault() : null;
            var currentSaveData = storedData != null ? JsonConvert.DeserializeObject<IList<NodeBackupData>>(storedData) : new List<NodeBackupData>();
            var existingRecord = currentSaveData.SingleOrDefault(x => x.RootId.Equals(parent.Id) && x.NodeName.Equals(nodeToBackup.Name));
            if (existingRecord != null)
            {
                existingRecord.NodeId = backupRecord.NodeId;
                existingRecord.NodeName = backupRecord.NodeName;
                existingRecord.RootId = backupRecord.RootId;
                existingRecord.RootName = backupRecord.RootName;
            }
            else
            {
                currentSaveData.Add(backupRecord);
            }
            File.WriteAllText("backupsFolderId.txt", JsonConvert.SerializeObject(currentSaveData));
            Console.WriteLine($"{nodeToBackup.Name} node Id stored");
        }

        private static string ReadBackupsNodeId(INode parent, string nodeName)
        {
            if (File.Exists("backupsFolderId.txt"))
            {
                Console.WriteLine("Looking in backups file");
                var savedData = JsonConvert.DeserializeObject<IList<NodeBackupData>>(File.ReadAllLines("backupsFolderId.txt").FirstOrDefault());
                var searchResult = savedData.SingleOrDefault(x => x.RootId.Equals(parent.Id) && x.NodeName.Equals(nodeName));
                if (searchResult != null)
                {
                    Console.WriteLine("Saved data found");
                    return searchResult.NodeId;
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