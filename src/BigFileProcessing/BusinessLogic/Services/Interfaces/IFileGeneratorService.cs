using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Interfaces
{
    public interface IFileGeneratorService
    {
        Task<BLO.Result<BLO.FileGenerationResponse>> GenerateAsync();
    }
}
