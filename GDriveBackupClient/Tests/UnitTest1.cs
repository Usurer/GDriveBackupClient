using System;
using System.IO;
using System.Threading;
using Autofac;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LocalFileSystemLib;
using GoogleDriveFileSystemLib;
using Uploader = GDriveClientLib.Implementations.Uploader;

namespace Tests
{
    [TestClass]
    public class LocalFileSystemTests
    {
        [TestMethod]
        public void TestLocalTree()
        {
            var fileManager = new LocalFileSystemLib.FileManager();
            var node = fileManager.GetTree(@"E:\Coding", 1);
            node = fileManager.GetTree(@"E:\Coding\Main.7z", 1);
        }

        [TestMethod]
        public void TestGoogleTree()
        {
            IContainer container = RegisterComponents();
            var fileManager = new GoogleDriveFileSystemLib.FileManager(container.Resolve<IGoogleDriveService>());
            var node = fileManager.GetTree("root", 2);
            Assert.IsNotNull(node);
        }

        public IContainer RegisterComponents()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Uploader>().SingleInstance().As<IUploader>();
            builder.RegisterType<Reader>().SingleInstance().As<IReader>();

            var service = InitializeDriveService();
            builder.RegisterInstance(service).SingleInstance().As<IGoogleDriveService>();

            return builder.Build();
        }

        private IGoogleDriveService InitializeDriveService()
        {
            UserCredential credential;
            using (var filestream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(filestream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("DriveCommandLineSample"))
                    .Result;
            }

            return new GoogleDriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });
        }
    }
}
