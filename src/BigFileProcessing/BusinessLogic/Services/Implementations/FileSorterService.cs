

using Microsoft.Extensions.Options;

using System.Diagnostics;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using BLC = BusinessLogic.Constants;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class FileSorterService : BLI.IFileSorterService
    {
        #region Private Members
        
        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly BLI.IFileSplitter _splitter;

        #endregion



        #region Constructors
        public FileSorterService(IOptions<INF.SorterOptions> sorterOptions, 
                                    BLI.IFileSplitter splitter)
        {
            _sorterOptions = sorterOptions;
            _splitter = splitter;
        }

        #endregion



        #region Public Methods
        public async Task<BLO.Result<BLO.FileSortResponse>> SortAsync()
        {
            string inputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}";
            string outputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}";

            try
            {
                var stopwatch = Stopwatch.StartNew();

                if (File.Exists(outputFileName) == true)
                {
                    File.Delete(outputFileName);
                }                

                var files = await _splitter.SplitInputFileAsync();

                stopwatch.Stop();

                var output = new BLO.FileSortResponse()
                {
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    TotalFiles = files.Count
                };

                return BLO.Result<BLO.FileSortResponse>.Success(output);
            }
            catch (Exception ex)
            {
                return BLO.Result<BLO.FileSortResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
