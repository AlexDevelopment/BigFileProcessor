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

        public async Task<BLO.Result<BLO.FileGenerationResponse>> GenerateAsync()
        {                       
            var process = Process.GetCurrentProcess();

            try
            {
                _logger.LogInformation("starting file generation operation.");

                long before = process.WorkingSet64;

                var stopwatch = Stopwatch.StartNew();

                string fileName = $"{_generatorOptions.Value.Folder}\\{BLC.Files.InputFile}";
                long number = 0;
                long writtenBytes = 0;

                if (File.Exists(fileName) == true)
                { 
                    File.Delete(fileName);
                }

                using (var writer = new StreamWriter(fileName,
                                                        append: false,
                                                        encoding: Encoding.UTF8,
                                                        bufferSize: 65536))
                {
                    while (true)
                    {
                        var row = _rowContentProvider.Generate();

                        long rowBytes = Encoding.UTF8.GetByteCount(row.ToString()) + 1;

                        if (writtenBytes + rowBytes > _generatorOptions.Value.MaxFileSize)
                        {
                            break;
                        }

                        await writer.WriteLineAsync(row.ToString());

                        number++;
                        writtenBytes += rowBytes;
                    }
                }

                stopwatch.Stop();

                long after = process.WorkingSet64;

                var output = new BLO.FileGenerationResponse()
                {
                    FileName = fileName,
                    TotalRecords = number, 
                    ElapsedTime = stopwatch.ElapsedMilliseconds,
                    UsedMemory = after - before,
                    SavedContentSize = writtenBytes
                };

                _logger.LogInformation("file generation operation completed successfully. {FileGenerationResponse}", output.ToLog());

                return BLO.Result<BLO.FileGenerationResponse>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred during the file generation operation.");

                return BLO.Result<BLO.FileGenerationResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
