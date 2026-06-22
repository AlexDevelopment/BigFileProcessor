
namespace BusinessLogic.Objects
{
    /// <summary>
    /// Represents the response of a file generation operation.
    /// </summary>
    public record FileGenerationResponse : IServiceResponse
    {
        #region Public Properties

        public required string FileName { get; init; }
        public required long TotalRecords { get; init; }
        public required long ElapsedTime { get; init; }
        public required long SavedContentSize { get; init; }

        #endregion



        #region Public Methods

        public string ToLog()
        {
            var minutes = TimeSpan.FromMilliseconds(ElapsedTime).TotalMinutes;

            return $"\nelapsed time: {ElapsedTime:N0} ms / {minutes:N2} min\ntotal records: {TotalRecords:N0}\nsaved content size: {SavedContentSize:N0} bytes\nfile name: {FileName}";
        }

        #endregion

    }
}
