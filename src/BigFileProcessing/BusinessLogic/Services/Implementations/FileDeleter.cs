using BLI = BusinessLogic.Services.Interfaces;

namespace BusinessLogic.Services.Implementations
{
    public class FileDeleter : BLI.IFileDeleter
    {
        #region Public Methods

        public Task DeleteFilesAsync(List<string> files)
        {
            foreach (var file in files)
            {
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
