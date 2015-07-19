using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;

namespace LocalFileSystemLib
{
    public class FileManager : IFileManager
    {
        public INode GetTree(string rootPath, int? depth)
        {
            if (depth <= 0)
            {
                return null;
            }

            var directoryInfo = new DirectoryInfo(rootPath);
            if (directoryInfo.Attributes != FileAttributes.Directory)
            {
                var file = new FileInfo(rootPath);
                return new Node
                {
                    Id = file.FullName,
                    Name = file.Name,
                    NodeType = NodeType.File,
                    Children = new INode[0],
                };
            }

            var result = new Node
            {
                Name = directoryInfo.Name,
                NodeType = NodeType.Folder
            };

            var tree = directoryInfo.EnumerateFileSystemInfos().Select(fileSystemInfo => GetTree(fileSystemInfo.FullName, depth - 1)).ToList();

            result.Children = tree.Where(x => x != null).ToArray();
            return result;
        }
    }
}
