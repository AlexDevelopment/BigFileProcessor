namespace Infrastructure
{
    public class GeneratorOptions : BaseOptions
    {
        #region Public constants

        public const string SectionName = "Generator";

        #endregion



        #region Public properties
        public required long MaxFileSize { get; set; }

        #endregion
    }
}
