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
        private readonly BLI.IFileDeleter _deleter;
        private readonly ILogger<FileSorterService> _logger;

        #endregion



        #region Constructors
        public FileSorterService(IOptions<INF.SorterOptions> sorterOptions, 
                                    BLI.IFileSplitter splitter,
                                    BLI.IFileMerger merger,
                                    BLI.IFileDeleter deleter,
                                    ILogger<FileSorterService> logger)
        {
            _sorterOptions = sorterOptions;
            _splitter = splitter;
            _merger = merger;
            _deleter = deleter;
            _logger = logger;
        }

        #endregion



        #region Public Methods
        public async Task<BLO.Result<BLO.FileSortResponse>> SortAsync()
        {
            long peak = 0;

            using var cts = new CancellationTokenSource();

            var memoryCheck = Task.Run(async () =>
            {
                while (cts.Token.IsCancellationRequested == false)
                {
                    long managed = GC.GetTotalMemory(false);

                    if (managed > peak)
                    {
                        peak = managed;
                    }

                    try { await Task.Delay(25, cts.Token); } catch { }
                }                
            });

            try
            {
                _logger.LogInformation("starting file sort operation...");

                var stopwatch = Stopwatch.StartNew();

                string inputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}";
                string outputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}";

                _logger.LogInformation("input file: {InputFileName}", inputFileName);
                _logger.LogInformation("output file: {OutputFileName}", outputFileName);    

                if (File.Exists(outputFileName) == true)
                {
                    File.Delete(outputFileName);
                }                

                _logger.LogInformation("starting file split operation...");

                var splitWatch = Stopwatch.StartNew();

                var files = await _splitter.SplitInputFileAsync();

                splitWatch.Stop();

                _logger.LogInformation("file split operation completed successfully. {FileCount} files created.", files.Count);
                
                _logger.LogInformation("starting file merge operation...");

                var mergeWatch = Stopwatch.StartNew();

                _merger.MergeFiles(files);

                mergeWatch.Stop();

                _logger.LogInformation("file merge operation completed successfully.");

                stopwatch.Stop();

                cts.Cancel();

                await memoryCheck;

                var output = new BLO.FileSortResponse()
                {
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    FileSplitElapsedTime = splitWatch.ElapsedMilliseconds,
                    FileMergeElapsedTime = mergeWatch.ElapsedMilliseconds,
                    UsedMemory = peak,
                    TotalFiles = files.Count,
                    OutputFileName = outputFileName,
                    ConsumerCount = _sorterOptions.Value.ConsumerCount,
                    ChannelCapacity = _sorterOptions.Value.ChannelCapacity
                };
                
                //the file deletion is outside of time measurement, as it is not part of the sorting operation

                _logger.LogInformation("starting file delete operation...");

                await _deleter.DeleteFilesAsync(files);

                _logger.LogInformation("file delete operation completed successfully.");

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
