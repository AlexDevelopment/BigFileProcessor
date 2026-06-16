namespace Services
{
    public interface IFileGeneratorService
    {
        Task<Result<FileGenerationResponse>> GenerateAsync(FileGenerationRequest request);
    }
}
