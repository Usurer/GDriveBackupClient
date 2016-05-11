using System;
using System.Collections.Generic;
using System.Linq;
using GDriveClientLib.Abstractions;

namespace GDriveBackupClient
{
    internal class TreeNavigator
    {
        private Stack<INode> GoogleStack { get; set; }
        private Stack<INode> LocalStack { get; set; }
        private IFileManager GoogleManager { get; set; }
        private IFileManager LocalManager { get; set; }
        private FileCacheManager FileCacheManager { get; set; }
            
        public TreeNavigator(INode googleRoot, INode localRoot, IFileManager googleFileManager, IFileManager localFileManager, FileCacheManager fileCacheManager)
        {
            GoogleStack = new Stack<INode>();    
            LocalStack = new Stack<INode>();
            GoogleStack.Push(googleRoot);
            LocalStack.Push(localRoot);
            GoogleManager = googleFileManager;
            LocalManager = localFileManager;
            FileCacheManager = fileCacheManager;
        }

        public INode GetCurrentGoogleNode()
        {
            return GoogleStack.Peek();
        }

        public INode GetCurrentLocalNode()
        {
            return LocalStack.Peek();
        }

        public void NavigateBothTreesUp()
        {
            if (GoogleStack.Count <= 1 || LocalStack.Count <= 1)
            {
                Console.WriteLine($"Cannot go UP in {(GoogleStack.Count <= 1 ? "Google" : "Local or both")}");
            }
            else
            {
                GoogleStack.Pop();
                LocalStack.Pop();
            }
        }

        public void NavigateBothTreesDown(string nodeName)
        {
            if (GoogleStack.Peek() == null || LocalStack.Peek() == null)
            {
                Console.WriteLine($"Skip searching because current root doesn't exist in {(GoogleStack.Peek() == null ? "Google" : "Local or both")}");

            }
            else
            {
                var googleNode = FindFolderInGoogle(nodeName);
                GoogleStack.Push(googleNode);
                var localNode = FindFolderLocally(nodeName);
                LocalStack.Push(localNode);
            }
        }

        private INode FindFolderInGoogle(string name)
        {
            return FindChildByName(GoogleStack, GoogleManager, name);
        }

        private INode FindFolderLocally(string name)
        {
            return FindChildByName(LocalStack, LocalManager, name);
        }

        private INode FindChildByName(Stack<INode> stack, IFileManager manager, string name)
        {
            var currentRoot = stack.Peek();
            if (currentRoot == null)
            {
                return null;
            }

            var cachedId = FileCacheManager.GetIdByNameAndParent(name, currentRoot.Id);
            if (!string.IsNullOrEmpty(cachedId))
            {
                return manager.GetTree(cachedId);
            }

            var currentRootChildren = currentRoot.Children ?? new List<INode>();

            foreach (var child in currentRootChildren)
            {
                FileCacheManager.AddOrUpdate(child, currentRoot.Id);
                if (child.Name.Equals(name))
                {
                    return child;
                }
            }
            return null;
        }
    }
}