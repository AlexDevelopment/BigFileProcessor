namespace Infrastructure
{
    public class SorterOptions : BaseOptions
    {
        #region Public constants

        public const string SectionName = "Sorter";

        #endregion



        #region Public properties
        public required int MaxChunkSize { get; set; }
        public required int ChannelCapacity { get; set; }
        public int ConsumerCount { get; set; }

        #endregion
    }
}
