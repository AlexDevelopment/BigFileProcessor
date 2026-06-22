using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Filters;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;
using BLC = BusinessLogic.Constants;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class FileSplitter : BLI.IFileSplitter
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly BLI.IChunkFileNameComposer _chunkFileNameComposer;

        private readonly ILogger<FileSplitter> _logger;

        #endregion



        #region Constructors

        public FileSplitter(IOptions<INF.SorterOptions> sorterOptions, 
                                BLI.IChunkFileNameComposer chunkFileNameComposer,
                                ILogger<FileSplitter> logger)
        {
            _sorterOptions = sorterOptions;
            _chunkFileNameComposer = chunkFileNameComposer;
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

            int fileIndex = 0;
            long currentFileSize = 0;

            string chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);

            var rows = new List<string>();

            using (var reader = new StreamReader($"{folder}\\{BLC.Files.InputFile}",
                                                        Encoding.UTF8, false,
                                                        BLC.StreamBuffers.ReadBufferSize))
            {
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    token.ThrowIfCancellationRequested();

                    long rowSize = Encoding.UTF8.GetByteCount(line) + 1;

                    if (currentFileSize + rowSize > maxChunkSize && rows.Count > 0)
                    {
                        chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);

                        await channelWriter.WriteAsync(
                            new BLO.ChannelChunkData(chunkFileName, rows));

                        _logger.LogInformation("producer sent chunk: {ChunkFileName} / {RowCount:N0} rows", chunkFileName, rows.Count);

                        rows = new List<string>();
                        fileIndex++;
                        currentFileSize = 0;
                    }

                    rows.Add(line);
                    currentFileSize += rowSize;
                }
            }

            if (rows.Count > 0)
            {
                chunkFileName = _chunkFileNameComposer.GetFullFileName(folder, fileIndex);

                await channelWriter.WriteAsync(
                    new BLO.ChannelChunkData(chunkFileName, rows));

                _logger.LogInformation("producer sent chunk: {ChunkFileName} / {RowCount} rows", chunkFileName, rows.Count);
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
                    var rows = new List<BLO.RowData>();

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
