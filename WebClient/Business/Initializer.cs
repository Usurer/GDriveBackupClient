using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Autofac;
using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace WebClient.Business
{
    public class Initializer
    {
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