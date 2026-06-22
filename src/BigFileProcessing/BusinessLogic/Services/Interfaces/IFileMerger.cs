
namespace BusinessLogic.Services.Interfaces
{
    public interface IFileMerger
    {
        void MergeFiles(List<string> files, CancellationToken token);
    }
}
