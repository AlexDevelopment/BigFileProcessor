namespace Infrastructure
{
    public class GeneratorOptions
    {
        #region Constructor

        public GeneratorOptions()
        { 
        }

        #endregion



        #region Public constants

        public const string SectionName = "Generator";

        #endregion



        #region Public properties
        public string Folder { get; set; }
        public int NumberOfRecords { get; set; }

        #endregion
    }
}
