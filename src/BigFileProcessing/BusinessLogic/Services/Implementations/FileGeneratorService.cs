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

        private readonly BLI.IFileContentProvider _fileContentProvider;
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;

        #endregion



        #region Constructors
        public FileGeneratorService(BLI.IFileContentProvider fileContentProvider, 
                                    IOptions<INF.GeneratorOptions> generatorOptions)
        {
            _fileContentProvider = fileContentProvider;
            _generatorOptions = generatorOptions;
        }

        #endregion



        #region Public Methods

        public async Task<BLO.Result<BLO.FileGenerationResponse>> GenerateAsync()
        {
            string fileName = $"{_generatorOptions.Value.Folder}\\{BLC.Files.InputFile}";
            int number = 0;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                if (File.Exists(fileName) == true)
                { 
                    File.Delete(fileName);
                }

                using (var writer = new StreamWriter(fileName,
                                                        append: false,
                                                        encoding: Encoding.UTF8,
                                                        bufferSize: 65536))
                {
                    var response = _fileContentProvider.Generate();

                    await writer.WriteLineAsync(response.Content);

                    number = response.TotalRecords;
                }

                stopwatch.Stop();

                var output = new BLO.FileGenerationResponse()
                {
                    FileName = fileName,
                    NumberOfRecords = number, 
                    ElapsedTime = stopwatch.ElapsedMilliseconds
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
