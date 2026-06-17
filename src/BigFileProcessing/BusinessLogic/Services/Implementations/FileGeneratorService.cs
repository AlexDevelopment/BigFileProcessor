using System.Text;

using Microsoft.Extensions.Options;

using BusinessLogic.Objects;
using BusinessLogic.Services.Interfaces;

using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class FileGeneratorService : IFileGeneratorService
    {
        #region Private Members

        private readonly IFileContentProvider _fileContentProvider;
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;

        #endregion



        #region Constructors
        public FileGeneratorService(IFileContentProvider fileContentProvider, 
                                    IOptions<INF.GeneratorOptions> generatorOptions)
        {
            _fileContentProvider = fileContentProvider;
            _generatorOptions = generatorOptions;
        }

        #endregion



        #region Public Methods

        public async Task<Result<FileGenerationResponse>> GenerateAsync()
        {
            string fileName = $"{_generatorOptions.Value.Folder}\\not_sorted" + $".txt";
            int number = 0;

            try
            {
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

                var output = new FileGenerationResponse()
                {
                    FileName = fileName,
                    NumberOfRecords = number
                };

                return Result<FileGenerationResponse>.Success(output);
            }
            catch (Exception ex)
            {
                return Result<FileGenerationResponse>.Failure(ex);
            }
        }

        #endregion
    }
}
