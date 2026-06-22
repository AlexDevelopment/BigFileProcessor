using BLI = BusinessLogic.Services.Interfaces;

namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Composes file names for chunk files.
    /// </summary>
    public class ChunkFileNameComposer : BLI.IChunkFileNameComposer
    {
        #region Public Methods

        public string GetFullFileName(string folder, int chunkIndex)
        {
            return Path.Combine(folder, $"chunk_{chunkIndex}.txt");
        }

        #endregion
    }
}
