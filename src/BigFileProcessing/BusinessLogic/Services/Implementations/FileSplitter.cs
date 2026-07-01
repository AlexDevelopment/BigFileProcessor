using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Splits a large input file into smaller chunks for processing.
    /// </summary>
    public class FileSplitter : BLI.IFileSplitter
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;        
        private readonly BLI.IInputFileReader _chunkReader;
        private readonly ILogger<FileSplitter> _logger;

        #endregion



        #region Constructors

        public FileSplitter(IOptions<INF.SorterOptions> sorterOptions, 
                                BLI.IInputFileReader chunkReader,
                                ILogger<FileSplitter> logger)
        {
            _sorterOptions = sorterOptions;
            _chunkReader = chunkReader;
            _logger = logger;
        }

        #endregion



        #region Public Methods

        public async Task<List<string>> SplitInputFileAsync(CancellationToken token)
        {
            int consumerCount = Math.Max(1, _sorterOptions.Value.ConsumerCount);
            int capacity = Math.Max(1, _sorterOptions.Value.ChannelCapacity);

            var channel = Channel.CreateBounded<BLO.ChannelChunkData>(
                new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleWriter = true,
                    SingleReader = consumerCount == 1
                });

            var chunkFiles = new ConcurrentBag<string>();

            var consumers = new Task[consumerCount];

            for (int i = 0; i < consumerCount; i++)
            {
                consumers[i] = Task.Run(() => ConsumeAsync(channel.Reader, chunkFiles, _logger, token));
            }

            _logger.LogInformation("starting file split operation with {ConsumerCount:N0} consumers and channel capacity of {ChannelCapacity:N0}...", consumers.Length, capacity);

            try
            {
                await ProduceAsync(channel.Writer, token);

                channel.Writer.Complete();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("file split operation was canceled.");

                channel.Writer.Complete(ex);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "file split operation encountered an error: {ErrorMessage}", ex.Message);

                channel.Writer.Complete(ex);
            }
            finally
            {
                await Task.WhenAll(consumers);
            }

            return chunkFiles.ToList();
        }

        #endregion



        #region Private Methods

        private async Task ProduceAsync(ChannelWriter<BLO.ChannelChunkData> channelWriter, CancellationToken token)
        {
            string folder = _sorterOptions.Value.Folder;
            long maxChunkSize = _sorterOptions.Value.MaxChunkSize;
                      
            string inputFileName = Path.Combine(folder, BLC.Files.InputFile);

            foreach (var chunk in _chunkReader.ReadChunks(inputFileName, folder, maxChunkSize, token))
            {
                await channelWriter.WriteAsync(chunk, token);
            }
        }                

        private static async Task ConsumeAsync(ChannelReader<BLO.ChannelChunkData> channelReader,
                                               ConcurrentBag<string> chunkFiles,
                                               ILogger<FileSplitter> logger,
                                               CancellationToken token)
        {
            var parser = new RowDataParser();
            var comparer = new RowDataComparer();

            try
            {
                await foreach (var chunk in channelReader.ReadAllAsync(token))
                {
                    var rows = new List<BLO.RowData>(chunk.Rows.Count);

                    chunk.Rows.ForEach(line =>
                    {
                        token.ThrowIfCancellationRequested();

                        var row = parser.Parse(line);

                        if (row != null)
                        {
                            rows.Add((BLO.RowData)row);
                        }
                    });

                    rows.Sort(comparer);

                    using (var writer = new StreamWriter(chunk.FullFileName, false, Encoding.UTF8,
                                                         BLC.StreamBuffers.WriteBufferSize))
                    {
                        foreach (var row in rows)
                        {
                            token.ThrowIfCancellationRequested();

                            writer.WriteLine(row.Original);
                        }
                    }

                    chunkFiles.Add(chunk.FullFileName);

                    logger.LogInformation("consumer processed chunk: {ChunkFileName} => {RowCount:N0} rows", chunk.FullFileName, rows.Count);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("consumer operation was canceled.");

                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "consumer encountered an error: {ErrorMessage}", ex.Message);

                throw;
            }

        }


        #endregion
    }
}
