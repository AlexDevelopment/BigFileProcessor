
using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileSorterService
    {
        BLO.Result<BLO.FileSortResponse> Sort();
    }
}
