using System.Text;
using System.Diagnostics;

using Microsoft.Extensions.Options;

using BLO = BusinessLogic.Objects;
using BLI = BusinessLogic.Services.Interfaces;
using BLC = BusinessLogic.Constants;

using INF = Infrastructure;



namespace BusinessLogic.Services.Implementations
{
    public class FileGeneratorService : BLI.IFileGeneratorService
    {
        #region Private Members

        private readonly BLI.IRowContentProvider _rowContentProvider;
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;

        #endregion



        #region Constructors
        public FileGeneratorService(BLI.IRowContentProvider rowContentProvider,
                                    IOptions<INF.GeneratorOptions> generatorOptions)
        {
            _rowContentProvider = rowContentProvider;
            _generatorOptions = generatorOptions;
        }

        #endregion



        #region Public Methods

        public async Task<BLO.Result<BLO.FileGenerationResponse>> GenerateAsync()
        {                       
            var process = Process.GetCurrentProcess();

            try
            {
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

                        long rowBytes = Encoding.UTF8.GetByteCount(row) + 1;

                        if (writtenBytes + rowBytes > _generatorOptions.Value.MaxFileSize)
                        {
                            break;
                        }

                        await writer.WriteLineAsync(row);

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

                return BLO.Result<BLO.FileGenerationResponse>.Success(output);
            }
            catch (Exception ex)
            {
                return BLO.Result<BLO.FileGenerationResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
