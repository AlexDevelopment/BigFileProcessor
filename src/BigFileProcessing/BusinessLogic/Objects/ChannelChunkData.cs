namespace BusinessLogic.Objects
{
    /// <summary>
    /// A chunk of rows read from the input file, not yet sorted.
    /// The producer pre-assigns <see cref="FullFileName"/> (in read order) and hands
    /// ownership of <see cref="Rows"/> over to a consumer through the channel.
    /// </summary>
    public sealed record ChannelChunkData(string FullFileName, List<string> Rows);
}
