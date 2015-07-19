using GDriveClientLib.Abstractions;
using Google.Apis.Drive.v2;

namespace GDriveClientLib.Implementations
{
    public class GoogleDriveService : DriveService, IGoogleDriveService
    {
        public GoogleDriveService(Initializer serviceInitializer) : base(serviceInitializer)
        {
        }
    }
}
