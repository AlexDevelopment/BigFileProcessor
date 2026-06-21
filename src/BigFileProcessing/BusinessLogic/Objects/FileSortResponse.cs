
namespace BusinessLogic.Objects
{
    public record FileSortResponse : IServiceResponse
    {
        #region Public Properties

        public required long ElapsedTime { get; init; }
        public required long UsedMemory { get; init; }
        public long TotalFiles { get; init; }
        public required string OutputFileName { get; init; }
        public required long FileSplitElapsedTime { get; init; }
        public required long FileMergeElapsedTime { get; init; }
        public required int ConsumerCount { get; init; }
        public required int ChannelCapacity { get; init; }

        #endregion



        #region Public Methods

        public string ToLog()
        {
            var minutes = TimeSpan.FromMilliseconds(ElapsedTime).TotalMinutes;
            var splitMinutes = TimeSpan.FromMilliseconds(FileSplitElapsedTime).TotalMinutes;
            var mergeMinutes = TimeSpan.FromMilliseconds(FileMergeElapsedTime).TotalMinutes;

            return $"\noutput file name: {OutputFileName}\ntotal elapsed time: {ElapsedTime:N0} ms / {minutes:N2} min\nfile split time: {FileSplitElapsedTime:N0} ms / {splitMinutes:N2} min\nfile merge time: {FileMergeElapsedTime:N0} ms / {mergeMinutes:N2} min\nused memory: {UsedMemory:N0} bytes\ntotal files: {TotalFiles:N0}\nconsumer count: {ConsumerCount:N0}\nchannel capacity: {ChannelCapacity:N0}";
        }

        #endregion
    }
}
