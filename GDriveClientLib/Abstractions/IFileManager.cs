namespace GDriveClientLib.Abstractions
{
    public interface IFileManager
    {
        INode GetTree(string rootPath, int? depth);
    }
}