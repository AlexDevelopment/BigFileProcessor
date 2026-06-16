namespace Services
{
    public interface IFileSorterService
    {
        Task<Result<FileSortResponse>> SortAsync(FileSortRequest request);
    }
}
