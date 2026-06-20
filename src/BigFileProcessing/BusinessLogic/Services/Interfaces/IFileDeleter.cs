

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileDeleter
    {
        Task DeleteFilesAsync(List<string> files);
    }
}
