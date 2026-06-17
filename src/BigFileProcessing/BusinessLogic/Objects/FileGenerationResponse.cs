
namespace BusinessLogic.Objects
{
    public record FileGenerationResponse : IServiceResponse
    {
        public required string FileName { get; init; }
        public required int NumberOfRecords { get; init; }
        public required long ElapsedTime { get; init; }
    }
}
