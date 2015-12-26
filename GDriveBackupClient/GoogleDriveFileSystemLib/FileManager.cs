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

        // TODO: Move to config
        private const int PageSize = 100;

        public FileManager(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
        }

        public INode GetTree(string rootPath)
        {
            var requestResult = GetAllChildren(rootPath);

            var infoResult = GetFileInfo(rootPath);

            var result = new Node
            {
                Id = infoResult.Id,
                Name = infoResult.Title,
                NodeType = GetNodeType(infoResult),
                // I want children to be populated either by user request or in some background thread as this is very time-consuming operation
                Children = requestResult
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

        private List<ChildReference> GetAllChildren(string rootPath)
        {
            var result = new List<ChildReference>();
            var firstPageResult = GetChildrenPage(rootPath, null, PageSize);
            result.AddRange(firstPageResult.Items);

            var token = firstPageResult.NextPageToken;
            while (!string.IsNullOrEmpty(token))
            {
                var nextPageResult = GetChildrenPage(rootPath, token, PageSize);

                if (token == nextPageResult.NextPageToken)
                {
                    // This exception shouldn't appear at all.
                    throw new Exception("Uhh... going into neverending loop.");
                }

                token = nextPageResult.NextPageToken;
                result.AddRange(nextPageResult.Items);
            }

            return result;
        }

        private ChildList GetChildrenPage(string rootPath, string nextPageToken, int maxResults)
        {
            var request = GoogleDriveService.Children.List(rootPath);
            request.MaxResults = maxResults;
            request.PageToken = nextPageToken;
            return request.Execute();
        }

        private NodeType GetNodeType(File file)
        {
            return file.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.InvariantCultureIgnoreCase) 
                ? NodeType.Folder
                : NodeType.File;
        }
    }
}
