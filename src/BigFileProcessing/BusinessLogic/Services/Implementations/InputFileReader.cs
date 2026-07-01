using System.Text;

using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// the class to read the input file line by line and return the set of chunks
    /// </summary>
    public sealed class InputFileReader : BLI.IInputFileReader
    {
        #region Private members

        private readonly BLI.IChunkFileNameComposer _chunkFileNameComposer;

        #endregion



        #region Public constructor
        public InputFileReader(BLI.IChunkFileNameComposer chunkFileNameComposer)
        { 
            _chunkFileNameComposer = chunkFileNameComposer;
        }

        #endregion



        #region Public methods

        public IEnumerable<BLO.ChannelChunkData> ReadChunks(string inputFileName, string folder, 
                                                            long maxChunkSize, CancellationToken token)
        {
            var rows = new List<string>();
            int fileIndex = 0;
            string chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);

            using (var reader = new StreamReader(inputFileName,
                                            Encoding.UTF8, false,
                                            BLC.StreamBuffers.ReadBufferSize))
            {
                string? line;
                long currentFileSize = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    token.ThrowIfCancellationRequested();

                    long rowSize = Encoding.UTF8.GetByteCount(line) + 1;

                    if (currentFileSize + rowSize > maxChunkSize && rows.Count > 0)
                    {
                        yield return new BLO.ChannelChunkData(chunkFileName, rows);

                        rows = new List<string>(rows.Count);
                        currentFileSize = 0;
                        fileIndex++;
                        chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);
                    }

                    rows.Add(line);
                    currentFileSize += rowSize;
                }
            }

            if (rows.Count > 0) 
            {
                chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);
                yield return new BLO.ChannelChunkData(chunkFileName, rows);
            }
        }

        #endregion
    }
}
