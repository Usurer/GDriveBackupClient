using System;
using System.IO;
using GDriveClientLib.Interfaces;
using Google.Apis.Drive.v2;

namespace GDriveClientLib.Implementations
{
    public class Uploader : IUploader
    {
        private IGoogleDriveService GoogleDriveService { get; set; }

        public Uploader(IGoogleDriveService googleDriveService)
        {
            GoogleDriveService = googleDriveService;
        }
        
        public void Upload(Stream fileStream)
        {
            var body = new Google.Apis.Drive.v2.Data.File();
            body.Title = "My document 123";
            body.Description = "A test document";
            body.MimeType = "text/plain";

            FilesResource.InsertMediaUpload request = GoogleDriveService.Files.Insert(body, fileStream, "text/plain");
            request.Upload();

            Google.Apis.Drive.v2.Data.File file = request.ResponseBody;
            Console.WriteLine("File id: " + file.Id);
            Console.WriteLine("Press Enter to end this process.");
            Console.ReadLine();
        }
    }
}