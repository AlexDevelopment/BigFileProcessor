
using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileSorterService
    {
        Task<BLO.Result<BLO.FileSortResponse>> SortAsync(CancellationToken token);
    }
}
