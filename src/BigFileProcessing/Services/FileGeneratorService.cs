using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FileGeneratorService : IFileGeneratorService
    {
        #region Public Methods

        public Task<Result<FileGenerationResponse>> GenerateAsync(FileGenerationRequest request)
        {
            string fileName = $"init_{DateTime.Now:yyyyMMddHHmmss}.txt";

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
