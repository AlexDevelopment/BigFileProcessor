
namespace BusinessLogic.Objects
{
    public record FileSortResponse : IServiceResponse
    {
        public long ElapsedTime { get; init; }
    }
}
