using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Interfaces
{
    public interface IInputFileReader
    {
        IEnumerable<BLO.ChannelChunkData> ReadChunks(string inputFileName, string folder, 
                                                        long maxChunkSize, CancellationToken token);
    }
}