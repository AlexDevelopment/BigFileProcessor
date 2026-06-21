using BLI = BusinessLogic.Services.Interfaces;

namespace BusinessLogic.Services.Implementations
{
    public class ChunkFileNameComposer : BLI.IChunkFileNameComposer
    {
        #region Public Methods

        public string GetFullFileName(string folder, int chunkIndex)
        {
            return $"{folder}\\chunk_{chunkIndex}.txt";
        }

        #endregion
    }
}
