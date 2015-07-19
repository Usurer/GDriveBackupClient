using System.IO;

namespace GDriveClientLib.Abstractions
{
    public interface IUploader
    {
        void Upload(Stream fileStream);
    }
}
