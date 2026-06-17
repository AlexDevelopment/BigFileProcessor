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

        public Task<Result<FileGenerationResponse>> GenerateAsync(FileGenerationRequest request)
        {
            string fileName = $"{_generatorOptions.Value.Folder}\\not_sorted" + $".txt";

            using (var writer = new StreamWriter(fileName,
                                                    append: false,
                                                    encoding: Encoding.UTF8,
                                                    bufferSize: 65536))
            {
                var content = _fileContentProvider.Generate();

                writer.WriteLine(content);
            }

            var response = new FileGenerationResponse()
            {
                FileName = fileName,
                NumberOfRecords = request.NumberOfRecords
            };

            return Task.FromResult(Result<FileGenerationResponse>.Success(response));
        }

        #endregion
    }
}
