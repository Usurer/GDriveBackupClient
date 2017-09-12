using System.Threading.Tasks;

namespace GDriveClientLib.Abstractions
{
    public interface IFileManager
    {
        Task<INode> GetTree(string rootPath);
    }
}