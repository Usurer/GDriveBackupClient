using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Requests;
using log4net;

namespace GoogleDriveFileSystemLib
{
    public class FileManager : IFileManager
    {
        private IGoogleDriveService GoogleDriveService { get; set; }
        private ILog Log { get; set; }

        // TODO: Move to config
        private const int PageSize = 100;

        public FileManager(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
            Log = log4net.LogManager.GetLogger(typeof(FileManager));
        }

        public async Task<INode> GetTree(string rootPath)
        {
            var requestResult = await GetAllChildren(rootPath);

            var infoResult = GetFileInfo(rootPath);

            var batchRequest = new BatchRequest(GoogleDriveService as DriveService);
            var files = new List<File>();
            var errors = new List<RequestError>();

            foreach (var child in requestResult)
            {
                var request = GoogleDriveService.Files.Get(child.Id);
                batchRequest.Queue<File>(request, (content, error, index, message) =>
                {
                    if (error != null)
                    {
                        errors.Add(error);
                    }
                    files.Add(content);
                });
            }

            await batchRequest.ExecuteAsync();

            if (errors.Any())
            {
                Log.Debug($"Batch Request errors: {Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(x => x.Message))}");
            }

            if (files.Any(x => x == null))
            {
                throw new Exception($"List of files contains {files.Count(x => x == null)} NULL values of total {files.Count} elements. WTF?!");
            }

            var result = new Node
            {
                Id = infoResult.Id,
                Name = infoResult.Title,
                NodeType = GetNodeType(infoResult),
                // I want children to be populated either by user request or in some background thread as this is very time-consuming operation
                //Children = asyncChildren.Select(x => x.Result).Select(x => new Node {Id = x.Id, Name = x.Name.Title}).ToArray(),
                Children = files.Select(x => new Node { Id = x.Id, Name = x.Title }).ToArray(),
            };

            return result;
        }

        private File GetFileInfo(string rootPath)
        {
            var infoRequest = GoogleDriveService.Files.Get(rootPath);
            var infoResult = infoRequest.Execute();
            return infoResult;
        }

        private async Task<File> GetFileInfoAsync(string rootPath)
        {
            var infoRequest = GoogleDriveService.Files.Get(rootPath);
            var infoResult = await infoRequest.ExecuteAsync();
            return infoResult;
        }

        private async Task<List<ChildReference>> GetAllChildren(string rootPath)
        {
            var result = new List<ChildReference>();
            var firstPageResult = await GetChildrenPage(rootPath, null, PageSize);
            result.AddRange(firstPageResult.Items);

            var token = firstPageResult.NextPageToken;
            while (!string.IsNullOrEmpty(token))
            {
                var nextPageResult = await GetChildrenPage(rootPath, token, PageSize);

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

        private async Task<ChildList> GetChildrenPage(string rootPath, string nextPageToken, int maxResults)
        {
            var request = GoogleDriveService.Children.List(rootPath);
            request.MaxResults = maxResults;
            request.PageToken = nextPageToken;
            return await request.ExecuteAsync();
        }

        private NodeType GetNodeType(File file)
        {
            return file.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.InvariantCultureIgnoreCase) 
                ? NodeType.Folder
                : NodeType.File;
        }
    }
}

