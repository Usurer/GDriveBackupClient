using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;

namespace GoogleDriveFileSystemLib
{
    public class FileManager : IFileManager
    {
        private IGoogleDriveService GoogleDriveService { get; set; }

        public FileManager(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
        }

        public INode GetTree(string rootPath)
        {
            var requestResult = GetChildren(rootPath);

            var infoResult = GetFileInfo(rootPath);

            var result = new Node
            {
                Id = infoResult.Id,
                Name = infoResult.Title,
                NodeType = GetNodeType(infoResult),
                // I want children to be populated either by user request or in some background thread as this is very time-consuming operation
                Children = requestResult
                    .Items
                    .Select(child => new Node { Id = child.Id }).ToArray(),
            };

            return result;
        }

        private File GetFileInfo(string rootPath)
        {
            var infoRequest = GoogleDriveService.Files.Get(rootPath);
            var infoResult = infoRequest.Execute();
            return infoResult;
        }

        private ChildList GetChildren(string rootPath)
        {
            var request = GoogleDriveService.Children.List(rootPath);
            request.MaxResults = 1000;
            var requestResult = request.Execute();
            return requestResult;
        }

        private NodeType GetNodeType(File file)
        {
            return file.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.InvariantCultureIgnoreCase) 
                ? NodeType.Folder
                : NodeType.File;
        }
    }
}
