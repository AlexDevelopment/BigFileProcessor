using BusinessLogic.Exceptions;
using Microsoft.Extensions.Options;
using System.Text;
using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class FileMerger : BLI.IFileMerger
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly List<StreamReader> _readers;
        private readonly BLI.IRowDataParser _rowDataParser;

        #endregion



        #region Constructors

        public FileMerger(IOptions<INF.SorterOptions> sorterOptions, BLI.IRowDataParser rowDataParser)
        {
            _sorterOptions = sorterOptions;
            _rowDataParser = rowDataParser;

            _readers = new List<StreamReader>();
        }

        #endregion



        #region Public Methods

        public async Task MergeFilesAsync(List<string> files)
        {
            StreamWriter writer = new StreamWriter(
                                        $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}",
                                        false, Encoding.UTF8);

            try
            { 
                foreach (var file in files)
                {
                    _readers.Add(new StreamReader(file));
                }

                var queue = new PriorityQueue<QueueItem, BLO.RowData>(new RowDataComparer());

                for (int i = 0; i < _readers.Count; i++)
                {
                    var line = await _readers[i].ReadLineAsync();

                    if (line == null) 
                    { 
                        continue;
                    }

                    var row = _rowDataParser.Parse(line);

                    queue.Enqueue(new QueueItem(Row: row, ReaderIndex: i), row);
                }

                while (queue.Count > 0)
                {
                    var item = queue.Dequeue();

                    await writer.WriteLineAsync(item.Row.ToString());

                    var nextLine = await _readers[item.ReaderIndex].ReadLineAsync();

                    if (nextLine != null)
                    {
                        var row = _rowDataParser.Parse(nextLine);

                        queue.Enqueue(new QueueItem(Row: row, ReaderIndex: item.ReaderIndex), row);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileMergerException(ex);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }

                foreach (var reader in _readers)
                {
                    reader.Dispose();
                    reader.Close();
                }
            }
        }

        #endregion


        public record struct QueueItem(BLO.RowData Row, int ReaderIndex);
        //public record struct LineComparison(string Text, int Number);
    }
}
