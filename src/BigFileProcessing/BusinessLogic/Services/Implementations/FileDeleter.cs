using BLI = BusinessLogic.Services.Interfaces;

namespace BusinessLogic.Services.Implementations
{
    public class FileDeleter : BLI.IFileDeleter
    {
        #region Public Methods

        public Task DeleteFilesAsync(List<string> files, CancellationToken token)
        {
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();

                if (File.Exists(file) == true)
                {
                    File.Delete(file);
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
