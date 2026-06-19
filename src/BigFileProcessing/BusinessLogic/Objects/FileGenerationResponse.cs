
namespace BusinessLogic.Objects
{
    public record FileGenerationResponse : IServiceResponse
    {
        #region Public Properties

        public required string FileName { get; init; }
        public required long TotalRecords { get; init; }
        public required long ElapsedTime { get; init; }
        public required long UsedMemory { get; init; }
        public required long SavedContentSize { get; init; }

        #endregion



        #region Public Methods

        public string ToLog()
        {
            return $"\nelapsed time: {ElapsedTime} ms\nused memory: {UsedMemory} bytes\ntotal records: {TotalRecords}\nsaved content size: {SavedContentSize} bytes\nfile name: {FileName}\n";
        }

        #endregion

    }
}
