using System.Collections.Generic;

namespace GDriveClientLib.Abstractions
{
    public interface INode
    {
        // Full path for local FS, Id for Google items
        string Id { get; set; }

        string Name { get; set; }

        NodeType NodeType { get; set; }

        IEnumerable<INode> Children { get; set; }                
    }
}