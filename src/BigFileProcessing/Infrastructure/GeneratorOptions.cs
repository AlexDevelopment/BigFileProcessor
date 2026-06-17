namespace Infrastructure
{
    public class GeneratorOptions : BaseOptions
    {
        #region Public constants

        public const string SectionName = "Generator";

        #endregion



        #region Public properties
        public required long MaxFileSize { get; set; }
        public required int[] Numbers { get; set; }
        public required string[] Strings { get; set; }

        #endregion
    }
}
