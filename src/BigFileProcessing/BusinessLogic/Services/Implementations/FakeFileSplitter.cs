using System.Text;

using Microsoft.Extensions.Options;

using BLI = BusinessLogic.Services.Interfaces;
using BLC = BusinessLogic.Constants;
using BLO = BusinessLogic.Objects;

using INF = Infrastructure;



namespace BusinessLogic.Services.Implementations
{
    public class FakeFileSplitter : BLI.IFileSplitter
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly BLI.IRowDataParser _parser;

        #endregion



        #region Constructors

        public FakeFileSplitter(IOptions<INF.SorterOptions> sorterOptions, BLI.IRowDataParser parser)
        {
            _sorterOptions = sorterOptions;
            _parser = parser;
        }

        #endregion



        #region Public Methods
        public List<string> SplitInputFile()
        {
            var files = Directory.EnumerateFiles(_sorterOptions.Value.Folder, "chunk_*.txt");

            return files.ToList();
        }

        #endregion
    }
}
