
namespace BusinessLogic.Objects
{
    /// <summary>
    /// Represents an item in the processing queue.
    /// </summary>
    public record struct QueueItem(RowData Row, int ReaderIndex);
}
