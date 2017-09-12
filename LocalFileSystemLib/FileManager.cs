﻿using System;
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
        public async Task<INode> GetTree(string rootPath)
        {
            var directoryInfo = new DirectoryInfo(rootPath);
            if (directoryInfo.Attributes != FileAttributes.Directory)
            {
                var file = new FileInfo(rootPath);
                return await new Task<INode>(() =>  new Node
                {
                    Id = file.FullName,
                    Name = file.Name,
                    NodeType = NodeType.File,
                    Children = new INode[0],
                }).ConfigureAwait(false);
            }

            var result = await new Task<INode>(() => new Node
            {
                Id = directoryInfo.FullName,
                Name = directoryInfo.Name,
                NodeType = NodeType.Folder
            }).ConfigureAwait(false);

            result.Children = directoryInfo.EnumerateFileSystemInfos().Select(fileSystemInfo => new Node { Id = fileSystemInfo.FullName, Name = fileSystemInfo.Name, Children = new INode[0] }).ToList();

            return result;
        }
    }
}
