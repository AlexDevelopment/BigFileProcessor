

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileSplitter
    {
        Task<List<string>> SplitInputFileAsync(CancellationToken token);
    }
}
