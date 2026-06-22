

namespace BusinessLogic.Objects
{
    /// <summary>
    /// Represents a response from a service operation.
    /// </summary>
    public interface IServiceResponse
    {
        long ElapsedTime { get; init; }
        string ToLog();
    }
}
