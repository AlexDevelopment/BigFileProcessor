using BusinessLogic.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileGeneratorService
    {
        Task<Result<FileGenerationResponse>> GenerateAsync(FileGenerationRequest request);
    }
}
