
namespace BusinessLogic.Objects
{
    public record FileSortResponse : IServiceResponse
    {
        public required long ElapsedTime { get; init; }
        public required long UsedMemory { get; init; }
        public long TotalFiles { get; init; }
    }
}
