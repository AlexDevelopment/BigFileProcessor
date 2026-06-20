

namespace BusinessLogic.Objects
{
    public interface IServiceResponse
    {
        long ElapsedTime { get; init; }
        long UsedMemory { get; init; }
        string ToLog();
    }
}
