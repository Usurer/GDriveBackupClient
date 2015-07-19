using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using Google.Apis.Drive.v2.Data;

namespace GoogleDriveFileSystemLib
{
    public class FileManager : IFileManager
    {
        private IGoogleDriveService GoogleDriveService { get; set; }

        public FileManager(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
        }

        public INode GetTree(string rootPath, int? depth)
        {
            if (depth <= 0)
            {
                return null;
            }

            var request = GoogleDriveService.Children.List(rootPath);
            var requestResult = request.Execute();

            var infoRequest = GoogleDriveService.Files.Get(rootPath);
            var infoResult = infoRequest.Execute();

            var result = new Node
            {
                Id = infoResult.Id,
                Name = infoResult.Title,
                NodeType = GetNodeType(infoResult),
                Children = requestResult
                    .Items
                    .Select(child => GetTree(child.Id, depth - 1))
                    .Where(x => x != null)
                    .ToArray(),
            };

            return result;
        }

        private NodeType GetNodeType(File file)
        {
            return file.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.InvariantCultureIgnoreCase) 
                ? NodeType.Folder
                : NodeType.File;
        }
    }
}
