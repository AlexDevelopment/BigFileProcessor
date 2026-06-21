namespace Infrastructure
{
    public class GeneratorOptions : BaseOptions
    {
        #region Public constants

        public const string SectionName = "Generator";

        #endregion



        #region Public properties

        /// <summary>
        /// The max file size in bytes
        /// </summary>
        public required long MaxFileSize { get; set; }

        /// <summary>
        /// The possible numbers to be used for a file row content generation
        /// </summary>
        public required int[] Numbers { get; set; }

        /// <summary>
        /// The possible strings to be used for a file row content generation
        /// </summary>
        public required string[] Strings { get; set; }

        /// <summary>
        /// The maximum number of text components in a file row.
        /// It affects the length of a row and the file size.
        /// </summary>
        public required int MaxTextComponentCount { get; set; }

        #endregion
    }
}
