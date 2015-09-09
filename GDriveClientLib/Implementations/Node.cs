using System.Collections.Generic;

using GDriveClientLib.Abstractions;

namespace GDriveClientLib.Implementations
{
    public class Node : INode
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public NodeType NodeType { get; set; }

        public IEnumerable<INode> Children { get; set; }
    }
}