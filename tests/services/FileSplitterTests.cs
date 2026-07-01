using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class FileSplitterTests
    {
        private static BLI.IFileSplitter Create(string folder, long maxChunkSize)
        {
            return new Impl.FileSplitter(
                OptionsFactory.Sorter(folder, maxChunkSize: maxChunkSize, channelCapacity: 4, consumerCount: 2),
                new Impl.InputFileReader(new Impl.ChunkFileNameComposer()),
                NullLogger<Impl.FileSplitter>.Instance);
        }

        // mirrors RowDataComparer: order by text (ordinal) then by number
        private static IEnumerable<string> ExpectedSort(IEnumerable<string> lines)
        {
            return lines
                .Select(l =>
                {
                    int dot = l.IndexOf('.');
                    return (line: l, number: int.Parse(l[..dot]), text: l[(dot + 2)..]);
                })
                .OrderBy(x => x.text, StringComparer.Ordinal)
                .ThenBy(x => x.number)
                .Select(x => x.line);
        }

        [Fact]
        public async Task SplitInputFileAsync_ProducesSortedChunksCoveringAllRows()
        {
            using var folder = new TempFolder();

            var input = new[]
            {
                "3. banana",
                "1. apple",
                "2. cherry",
                "5. apple",
                "4. date"
            };
            File.WriteAllLines(folder.File("unsorted.txt"), input);

            // small chunk size forces the input to be split across several files
            var files = await Create(folder.Path, maxChunkSize: 20).SplitInputFileAsync(CancellationToken.None);

            Assert.NotEmpty(files);
            Assert.True(files.Count > 1, "expected the input to be split into multiple chunks");
            Assert.All(files, f => Assert.True(File.Exists(f)));

            // every chunk is internally sorted
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                Assert.Equal(ExpectedSort(lines), lines);
            }

            // all original rows are present exactly once across all chunks
            var merged = files.SelectMany(File.ReadAllLines).ToList();
            Assert.Equal(input.OrderBy(x => x, StringComparer.Ordinal),
                         merged.OrderBy(x => x, StringComparer.Ordinal));
        }

        [Fact]
        public async Task SplitInputFileAsync_SingleConsumer_StillWorks()
        {
            using var folder = new TempFolder();

            File.WriteAllLines(folder.File("unsorted.txt"), new[] { "2. b", "1. a" });

            var splitter = new Impl.FileSplitter(
                OptionsFactory.Sorter(folder.Path, maxChunkSize: 1024, channelCapacity: 1, consumerCount: 1),
                new Impl.InputFileReader(new Impl.ChunkFileNameComposer()),
                NullLogger<Impl.FileSplitter>.Instance);

            var files = await splitter.SplitInputFileAsync(CancellationToken.None);

            Assert.Single(files);
            Assert.Equal(new[] { "1. a", "2. b" }, File.ReadAllLines(files[0]));
        }
    }
}
