
namespace BusinessLogic.Objects
{
    public record FileSortResponse : IServiceResponse
    {
        public long ElapsedTime { get; init; }
        public long TotalFiles { get; init; }
    }
}
