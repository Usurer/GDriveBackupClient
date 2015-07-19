using Google.Apis.Drive.v2;

namespace GDriveClientLib.Abstractions
{
    public interface IGoogleDriveService
    {
        FilesResource Files { get; }
        
        ChildrenResource Children { get; }
    }
}