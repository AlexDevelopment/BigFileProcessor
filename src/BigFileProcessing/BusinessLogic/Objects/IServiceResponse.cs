

namespace BusinessLogic.Objects
{
    public interface IServiceResponse
    {
        long ElapsedTime { get; init; }
        string ToLog();
    }
}
