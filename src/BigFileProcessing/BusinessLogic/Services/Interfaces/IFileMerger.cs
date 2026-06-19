
namespace BusinessLogic.Services.Interfaces
{
    public interface IFileMerger
    {
        Task MergeFilesAsync(List<string> files);
    }
}
