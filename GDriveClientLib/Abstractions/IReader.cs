namespace GDriveClientLib.Abstractions
{
    public interface IReader
    {
        void List();

        void ListChildren();

        void DrawRootTree();
    }
}
