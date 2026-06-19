
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
            var minutes = TimeSpan.FromMilliseconds(ElapsedTime).TotalMinutes;

            return $"\nelapsed time: {ElapsedTime:N0} ms / {minutes:N2} min\nused memory: {UsedMemory:N0} bytes\ntotal files: {TotalFiles:N0}\n";
        }

        #endregion
    }
}
