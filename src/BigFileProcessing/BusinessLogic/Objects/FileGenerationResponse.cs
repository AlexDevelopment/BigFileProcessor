
namespace BusinessLogic.Objects
{
    public record FileGenerationResponse : IServiceResponse
    {
        public required string FileName { get; init; }
        public required long TotalRecords { get; init; }
        public required long ElapsedTime { get; init; }
        public required long UsedMemory { get; init; }
        public required long SavedContentSize { get; init; }
    }
}
