using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using BLC = BusinessLogic.Constants;
using INF = Infrastructure;


namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Sorts large files by splitting them into smaller chunks, sorting each chunk, and then merging the sorted chunks.
    /// </summary>
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
        public async Task<BLO.Result<BLO.FileSortResponse>> SortAsync(CancellationToken token)
        {
            List<string> files = new List<string>();

            try
            {
                token.ThrowIfCancellationRequested();

                _logger.LogInformation("starting file sort operation...");

                var stopwatch = Stopwatch.StartNew();

                string inputFileName = Path.Combine(_sorterOptions.Value.Folder, BLC.Files.InputFile);
                string outputFileName = Path.Combine(_sorterOptions.Value.Folder, BLC.Files.OutputFile);

                _logger.LogInformation("input file: {InputFileName}", inputFileName);
                _logger.LogInformation("output file: {OutputFileName}", outputFileName);

                token.ThrowIfCancellationRequested();

                if (File.Exists(outputFileName) == true)
                {
                    File.Delete(outputFileName);
                }                

                _logger.LogInformation("starting file split operation...");

                var splitWatch = Stopwatch.StartNew();

                token.ThrowIfCancellationRequested();

                files = await _splitter.SplitInputFileAsync(token);

                splitWatch.Stop();

                _logger.LogInformation("file split operation completed successfully. {FileCount} files created.", files.Count);
                
                _logger.LogInformation("starting file merge operation...");

                var mergeWatch = Stopwatch.StartNew();

                token.ThrowIfCancellationRequested();

                _merger.MergeFiles(files, token);

                mergeWatch.Stop();

                _logger.LogInformation("file merge operation completed successfully.");

                stopwatch.Stop();

                var output = new BLO.FileSortResponse()
                {
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    FileSplitElapsedTime = splitWatch.ElapsedMilliseconds,
                    FileMergeElapsedTime = mergeWatch.ElapsedMilliseconds,
                    TotalFiles = files.Count,
                    OutputFileName = outputFileName,
                    ConsumerCount = _sorterOptions.Value.ConsumerCount,
                    ChannelCapacity = _sorterOptions.Value.ChannelCapacity
                };

                _logger.LogInformation("file sort operation completed successfully. {Output}", output.ToLog());

                return BLO.Result<BLO.FileSortResponse>.Success(output);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "file sort operation was cancelled.");

                return BLO.Result<BLO.FileSortResponse>.Cancel(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred during the file sort operation.");

                return BLO.Result<BLO.FileSortResponse>.Failure(ex);
            }
            finally
            {
                //chunk files are temporary and must be removed regardless of the outcome (success, failure or cancellation).
                //cleanup runs with CancellationToken.None so it always completes, and never throws so it cannot mask the result.

                if (files.Count > 0)
                {
                    try
                    {
                        _logger.LogInformation("starting file delete operation...");

                        await _deleter.DeleteFilesAsync(files, CancellationToken.None);

                        _logger.LogInformation("file delete operation completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "failed to delete temporary chunk files.");
                    }
                }
            }
        }

        #endregion
    }
}
