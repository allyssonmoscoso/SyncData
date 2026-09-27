using System.Threading.Tasks;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Interface for all file operations
    /// </summary>
    public interface IFileOperation
    {
        Task ExecuteAsync();
    }
}
