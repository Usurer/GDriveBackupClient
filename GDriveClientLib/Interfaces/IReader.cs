using Google.Apis.Drive.v2;

namespace GDriveClientLib.Interfaces
{
    public interface IReader
    {
        void List();

        void ListChildren();

        void DrawRootTree();
    }
}
