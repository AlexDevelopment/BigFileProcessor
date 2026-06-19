
namespace BusinessLogic.Objects
{
    public record FileSortResponse : IServiceResponse
    {
        #region Public Properties

        public required long ElapsedTime { get; init; }
        public required long UsedMemory { get; init; }
        public long TotalFiles { get; init; }

        #endregion



        #region Public Methods

        public string ToLog()
        {
            return $"\nelapsed time: {ElapsedTime} ms\nused memory: {UsedMemory} bytes\ntotal files: {TotalFiles}\n";
        }

        #endregion
    }
}
