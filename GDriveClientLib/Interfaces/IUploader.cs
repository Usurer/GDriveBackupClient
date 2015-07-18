using System.IO;
using Google.Apis.Drive.v2;

namespace GDriveClientLib.Interfaces
{
    public interface IUploader
    {
        void Upload(Stream fileStream);
    }
}
