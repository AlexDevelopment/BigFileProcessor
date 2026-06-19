using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly BLI.IFileMerger _merger;
        private readonly ILogger<FileSorterService> _logger;

        #endregion



        #region Constructors
        public FileSorterService(IOptions<INF.SorterOptions> sorterOptions, 
                                    BLI.IFileSplitter splitter,
                                    BLI.IFileMerger merger,
                                    ILogger<FileSorterService> logger)
        {
            _sorterOptions = sorterOptions;
            _splitter = splitter;
            _merger = merger;
            _logger = logger;
        }

        #endregion



        #region Public Methods
        public async Task<BLO.Result<BLO.FileSortResponse>> SortAsync()
        {
            var process = Process.GetCurrentProcess();

            try
            {
                _logger.LogInformation("starting file sort operation.");

                long before = process.WorkingSet64;

                var stopwatch = Stopwatch.StartNew();

                string inputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}";
                string outputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}";

                if (File.Exists(outputFileName) == true)
                {
                    File.Delete(outputFileName);
                }                

                var files = await _splitter.SplitInputFileAsync();

                await _merger.MergeFilesAsync(files);

                stopwatch.Stop();

                long after = process.WorkingSet64;

                var output = new BLO.FileSortResponse()
                {
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    UsedMemory = after - before,
                    TotalFiles = files.Count
                };

                _logger.LogInformation("file sort operation completed successfully. {Output}", output.ToLog());

                return BLO.Result<BLO.FileSortResponse>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred during the file sort operation.");

                return BLO.Result<BLO.FileSortResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
