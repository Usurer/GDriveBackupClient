using System;
using System.Collections.Generic;
using System.Linq;
using GDriveClientLib.Abstractions;
using Worker.Cache;
using Worker.Navigation;

namespace Worker
{
    public class Application
    {
        private IFileManager GoogleManager { get; set; }
        private IFileManager LocalManager { get; set; }
        private FileCacheManager FileCacheManager { get; set; }
        private MemoryCacheManager MemoryCacheManager { get; set; }

        public Application(IFileManager googleFileManager, IFileManager localFileManager, FileCacheManager fileCacheManager, MemoryCacheManager memoryCacheManager)
        {
            GoogleManager = googleFileManager;
            LocalManager = localFileManager;
            FileCacheManager = fileCacheManager;
            MemoryCacheManager = memoryCacheManager;
        }

        public void Start()
        {
            /*TODO: I want these Console calls to be done via some ILogger interface. In this class it's useless, but since I don't want
                Console to be used in other namespaces - I want them to receive the ILogger and just write stuff to Debug, Error etc log.
                So for Console app I'd be able to use simple class that would write to Console as an ILogger implementation.*/
                

            var backupsFolder = LoadGoogleBackupsFolder();
            var localDataRoot = LocalManager.GetTree(@"E:\CodeLearning\GDriveBackupClient\Data");

            PrintTree(backupsFolder, localDataRoot);

            var navigator = new TreeNavigator(backupsFolder, localDataRoot, GoogleManager, LocalManager, FileCacheManager);

            /*TODO: Here's one big todo. I have an issue with INodes children because sometimes I have only IDs there, and sometimes - full data.
             Another thing is that I use GetTree to get children names.
             So my assumption is - Children should _always_ have objects. Maybe I'd better hide list of IDs in some private property of a Node and Children collection would be empty until I'll fill it
             It's also worth to check if it's faster to ask Google for object info by ID - without getting any children info - I mean it could be useful for getting children names*/

            while (true)
            {
                Console.WriteLine("Enter a name of a folder to open or press enter to go up");
                var folderToOpen = Console.ReadLine();
                if (String.IsNullOrEmpty(folderToOpen))
                {
                    navigator.NavigateBothTreesUp();
                }
                else
                {
                    navigator.NavigateBothTreesDown(folderToOpen);
                }

                PrintTree(navigator.GetCurrentGoogleNode(), navigator.GetCurrentLocalNode());
            }
        }

        private void PrintTree(INode backupsFolder, INode localNode)
        {
            Console.WriteLine("=============");

            // TODO: Mind, that TreeNavigator already returns nodes filled with data and there is no need to call GetTree once again
            var googleFolderChildren = backupsFolder != null ? backupsFolder.Children.ToList() : new List<INode>();
            var localFolderChildren = localNode != null ? localNode.Children.ToList() : new List<INode>();

            var merge = googleFolderChildren.Concat(localFolderChildren.Where(x => googleFolderChildren.All(y => !y.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))));

            foreach (var child in merge)
            {
                var isInLocal = localFolderChildren.Any(x => x.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase));
                var isInGoogle = googleFolderChildren.Any(x => x.Name.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase));
                var isInBoth = isInLocal && isInGoogle;

                var symbolicPrefix = isInBoth ? "*" : isInLocal ? "L" : "G";

                Console.WriteLine($"{symbolicPrefix} {child.Name}");
            }

            Console.WriteLine("=============");
        }

        private INode LoadGoogleBackupsFolder(string parentId = "root", string folderName = "Backups")
        {
            Console.WriteLine("Searching for first AKA root level stuff");
            var cachedId = GetFromCache(folderName, parentId);
            if (!String.IsNullOrEmpty(cachedId))
            {
                return GoogleManager.GetTree(cachedId);
            }

            Console.WriteLine($"No saved data about {folderName} folder");
            var parent = GoogleManager.GetTree(parentId);

            var node = GetGoogleNodeByNameAndParent(parent, folderName);

            Console.WriteLine($"{folderName} loaded");
            AddToCache(node, parentId);
            return node;
        }

        private INode GetGoogleNodeByNameAndParent(INode parent, string name)
        {
            Console.WriteLine($"Getting google data for '{parent.Name}'");

            INode result = null;

            Console.WriteLine($"Enumerating {parent.Children.Count()} children, looking for {name} folder");
            foreach (var child in parent.Children)
            {
                Console.Write(".");

                if (child.Name == name)
                {
                    var childData = GoogleManager.GetTree(child.Id);

                    AddToCache(childData, parent.Id);
                    Console.WriteLine();
                    Console.WriteLine($"{name} folder found");
                    result = childData;
                    break;
                }
            }

            if (result == null)
            {
                throw new ApplicationException($"No folder named '{name}' found in Google Drive {parent.Name}");
            }

            return result;
        }

        private void AddToCache(INode nodeToBackup, string parentId)
        {
            FileCacheManager.AddOrUpdate(nodeToBackup, parentId);
        }

        private string GetFromCache(string nodeName, string parentId)
        {
            return FileCacheManager.GetIdByNameAndParent(nodeName, parentId);
        }
    }
}