using BusinessLogic.Exceptions;
using Microsoft.Extensions.Options;
using System.Text;
using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Merges multiple sorted files into a single output file.
    /// </summary>
    public class FileMerger : BLI.IFileMerger
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly BLI.IRowDataParser _rowDataParser;

        #endregion



        #region Constructors

        public FileMerger(IOptions<INF.SorterOptions> sorterOptions, BLI.IRowDataParser rowDataParser)
        {
            _sorterOptions = sorterOptions;
            _rowDataParser = rowDataParser;
        }

        #endregion



        #region Public Methods

        public void MergeFiles(List<string> files, CancellationToken token)
        {
            StreamWriter writer = new StreamWriter(
                                        $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}",
                                        false, Encoding.UTF8, BLC.StreamBuffers.WriteBufferSize);

            List<StreamReader> readers = new List<StreamReader>();

            try
            { 
                foreach (var file in files)
                {
                    readers.Add(new StreamReader(file, Encoding.UTF8, false, BLC.StreamBuffers.ReadBufferSize));
                }

                var queue = new PriorityQueue<BLO.QueueItem, BLO.RowData>(new RowDataComparer());

                for (int i = 0; i < readers.Count; i++)
                {
                    token.ThrowIfCancellationRequested();

                    var line = readers[i].ReadLine();

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
                    token.ThrowIfCancellationRequested();

                    var item = queue.Dequeue();

                    writer.WriteLine(item.Row.Original);

                    var nextLine = readers[item.ReaderIndex].ReadLine();

                    if (nextLine != null)
                    {
                        var row = _rowDataParser.Parse(nextLine);

                        if (row != null)
                        {
                            BLO.RowData realRow = (BLO.RowData)row;

                            queue.Enqueue(new BLO.QueueItem(Row: realRow, ReaderIndex: item.ReaderIndex), realRow);
                        }
                    }
                }
            }
            catch(OperationCanceledException)
            {
                throw;
            }   
            catch (Exception ex)
            {
                throw new FileMergerException(ex);
            }
            finally
            {
                writer?.Dispose();

                foreach (var reader in readers)
                {
                    reader?.Dispose();
                }

                readers.Clear();
            }
        }

        #endregion        
    }
}
