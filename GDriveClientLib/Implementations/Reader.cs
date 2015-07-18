using System;
using GDriveClientLib.Interfaces;
using Google.Apis.Drive.v2;

namespace GDriveClientLib.Implementations
{
    public class Reader : IReader
    {
        private IGoogleDriveService GoogleDriveService { get; set; }

        public Reader(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
        }

        public void List()
        {
            var request = GoogleDriveService.Files.List();
            var requsetResult = request.Execute();
            foreach (var item in requsetResult.Items)
            {
                Console.WriteLine(string.Format("Name '{0}' as {1}", item.Title, item.Description));
            }
            Console.WriteLine(requsetResult.Items.Count);
        }

        public void ListChildren()
        {
            var request = GoogleDriveService.Children.List("root");
            var requsetResult = request.Execute();
            foreach (var item in requsetResult.Items)
            {
                var infoRequest = GoogleDriveService.Files.Get(item.Id);
                var result = infoRequest.Execute();
                Console.WriteLine(result.Title);
                /*var childRequest = service.Children.Get(item.SelfLink, item.ChildLink);
                var res = childRequest.Execute();*/
            }
            
        }

        public void DrawRootTree()
        {
            DrawFolderTree("root", 1);
        }

        public void DrawFolderTree(string folderId, int lvl)
        {
            if (lvl > 3)
            {
                return;
            }
            var request = GoogleDriveService.Children.List(folderId);
            var result = request.Execute();
            var rootSubfolders = result.Items;
            var indent = string.Empty;
            for (int i = 0; i < lvl; i++)
            {
                indent = indent + "-";
            }
            foreach (var child in rootSubfolders)
            {
                Console.Write(indent);
                var infoRequest = GoogleDriveService.Files.Get(child.Id);
                var infoResult = infoRequest.Execute();
                Console.WriteLine(string.Format("{0} {1}", infoResult.Title, infoResult.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.InvariantCultureIgnoreCase) ? "*" : string.Empty));
                DrawFolderTree(child.Id, lvl + 1);
                Console.WriteLine();
            }
        }
    }
}