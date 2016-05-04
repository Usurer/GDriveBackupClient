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
            
        public TreeNavigator(INode googleRoot, INode localRoot, IFileManager googleFileManager, IFileManager localFileManager)
        {
            GoogleStack = new Stack<INode>();    
            LocalStack = new Stack<INode>();
            GoogleStack.Push(googleRoot);
            LocalStack.Push(localRoot);
            GoogleManager = googleFileManager;
            LocalManager = localFileManager;
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
            if (GoogleStack.Count == 0 || LocalStack.Count == 0)
            {
                Console.WriteLine($"Cannot go UP in {(GoogleStack.Count == 0 ? "Google" : "Local or both")}");
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
            return Expand(FindChildByName(GoogleStack, GoogleManager, name), GoogleManager);
        }

        private INode FindFolderLocally(string name)
        {
            return Expand(FindChildByName(LocalStack, LocalManager, name), LocalManager);
        }

        private INode FindChildByName(Stack<INode> stack, IFileManager manager, string name)
        {
            var currentRootChildren = stack.Peek()?.Children;
            return currentRootChildren
                ?.Select(child => manager.GetTree(child.Id))
                .FirstOrDefault(node => node.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        private INode Expand(INode nodeToExpand, IFileManager manager)
        {
            if (nodeToExpand == null)
            {
                return null;
            }

            foreach (var child in nodeToExpand.Children)
            {
                var fullNodeData = manager.GetTree(child.Id);
                child.Name = fullNodeData.Name;
            }
            return nodeToExpand;
        }
    }
}