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

        public void MergeFiles(List<string> files)
        {
            StreamWriter writer = new StreamWriter(
                                        $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}",
                                        false, Encoding.UTF8, BLC.StreamBuffers.WriteBufferSize);

            try
            { 
                foreach (var file in files)
                {
                    _readers.Add(new StreamReader(file, Encoding.UTF8, false, BLC.StreamBuffers.ReadBufferSize));
                }

                var queue = new PriorityQueue<BLO.QueueItem, BLO.RowData>(new RowDataComparer());

                for (int i = 0; i < _readers.Count; i++)
                {
                    var line = _readers[i].ReadLine();

                    if (line == null) 
                    { 
                        continue;
                    }

                    var row = _rowDataParser.Parse(line);

                    if (row == null)
                    {
                        continue;
                    }

                    BLO.RowData realRow = (BLO.RowData)row;

                    queue.Enqueue(new BLO.QueueItem(Row: realRow, ReaderIndex: i), realRow);
                }

                while (queue.Count > 0)
                {
                    var item = queue.Dequeue();

                    writer.WriteLine(item.Row.Original);

                    var nextLine = _readers[item.ReaderIndex].ReadLine();

                    if (nextLine != null)
                    {
                        var row = _rowDataParser.Parse(nextLine);

                        if (row == null)
                        {
                            continue;
                        }

                        BLO.RowData realRow = (BLO.RowData)row;

                        queue.Enqueue(new BLO.QueueItem(Row: realRow, ReaderIndex: item.ReaderIndex), realRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileMergerException(ex);
            }
            finally
            {
                writer?.Dispose();

                foreach (var reader in _readers)
                {
                    reader?.Dispose();
                }

                _readers.Clear();
            }
        }

        #endregion        
    }
}
