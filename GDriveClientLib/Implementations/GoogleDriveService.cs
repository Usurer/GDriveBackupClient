using Google.Apis.Drive.v2;
using GDriveClientLib.Interfaces;

namespace GDriveClientLib.Implementations
{
    public class GoogleDriveService : DriveService, IGoogleDriveService
    {
        public GoogleDriveService(Initializer serviceInitializer) : base(serviceInitializer)
        {
        }
    }
}
