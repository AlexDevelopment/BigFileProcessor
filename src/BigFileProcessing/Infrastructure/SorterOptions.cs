namespace Infrastructure
{
    public class SorterOptions : BaseOptions
    {
        #region Public constants

        public const string SectionName = "Sorter";

        #endregion



        #region Public properties
        public required int NumberOfChunks { get; set; }

        #endregion
    }
}
