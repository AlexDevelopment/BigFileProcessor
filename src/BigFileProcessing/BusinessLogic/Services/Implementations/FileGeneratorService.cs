using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;
using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using INF = Infrastructure;



namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Generates files with random content for testing purposes.
    /// </summary>
    public class FileGeneratorService : BLI.IFileGeneratorService
    {
        #region Private Members

        private readonly BLI.IRowContentProvider _rowContentProvider;
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;
        private readonly ILogger<FileGeneratorService> _logger;

        #endregion



        #region Constructors
        public FileGeneratorService(BLI.IRowContentProvider rowContentProvider,
                                    IOptions<INF.GeneratorOptions> generatorOptions, 
                                    ILogger<FileGeneratorService> logger)
        {
            _rowContentProvider = rowContentProvider;
            _generatorOptions = generatorOptions;
            _logger = logger;
        }

        #endregion



        #region Public Methods

        public async Task<BLO.Result<BLO.FileGenerationResponse>> GenerateAsync(CancellationToken token)
        {
            string fileName = $"{_generatorOptions.Value.Folder}\\{BLC.Files.InputFile}";

            try
            {
                _logger.LogInformation("starting file generation operation.");

                var stopwatch = Stopwatch.StartNew();

                long number = 0;
                long writtenBytes = 0;

                if (File.Exists(fileName) == true)
                { 
                    File.Delete(fileName);
                }

                using (var writer = new StreamWriter(fileName,
                                                        append: false,
                                                        encoding: Encoding.UTF8,
                                                        bufferSize: BLC.StreamBuffers.WriteBufferSize))
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        var row = _rowContentProvider.Generate();

                        long rowBytes = Encoding.UTF8.GetByteCount(row.ToArray()) + 1;

                        if (writtenBytes + rowBytes > _generatorOptions.Value.MaxFileSize)
                        {
                            break;
                        }

                        await writer.WriteLineAsync(row, token);

                        number++;
                        writtenBytes += rowBytes;
                    }
                }

                stopwatch.Stop();

                var output = new BLO.FileGenerationResponse()
                {
                    FileName = fileName,
                    TotalRecords = number, 
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    SavedContentSize = writtenBytes
                };

                _logger.LogInformation("file generation operation completed successfully. {FileGenerationResponse}", output.ToLog());

                return BLO.Result<BLO.FileGenerationResponse>.Success(output);
            }
            catch (OperationCanceledException ex)
            {
                if (File.Exists(fileName) == true)
                {
                    File.Delete(fileName);
                }

                _logger.LogWarning("file generation operation was canceled.");

                return BLO.Result<BLO.FileGenerationResponse>.Cancel(ex);
            }
            catch (Exception ex)
            {
                if (File.Exists(fileName) == true)
                {
                    File.Delete(fileName);
                }

                _logger.LogError(ex, "an error occurred during the file generation operation.");

                return BLO.Result<BLO.FileGenerationResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
