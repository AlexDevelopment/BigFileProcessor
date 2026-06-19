using BusinessLogic.Services.Implementations;
using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Extensions
{
    public static class StreamWriterExtension
    {
        public static StreamWriter SortWriteDispose(this StreamWriter writer, List<BLO.RowData> rows)
        {
            if (rows.Count == 0)
            { 
                return writer;
            }

            rows.Sort(new RowDataComparer());

            foreach (var row in rows)
            {
                writer.WriteLine(row.Output);
            }

            writer.Dispose();

            return writer;
        }
    }
}
