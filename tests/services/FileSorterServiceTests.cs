using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class FileSorterServiceTests
    {
        private static Impl.FileSorterService Create(
            string folder,
            BLI.IFileSplitter splitter,
            BLI.IFileMerger merger,
            BLI.IFileDeleter deleter,
            int consumerCount = 2,
            int channelCapacity = 4)
        {
            return new Impl.FileSorterService(
                OptionsFactory.Sorter(folder, consumerCount: consumerCount, channelCapacity: channelCapacity),
                splitter,
                merger,
                deleter,
                NullLogger<Impl.FileSorterService>.Instance);
        }

        [Fact]
        public async Task SortAsync_OrchestratesSplitMergeDeleteAndReportsResult()
        {
            using var folder = new TempFolder();

            var chunks = new List<string> { folder.File("chunk_0.txt"), folder.File("chunk_1.txt") };

            var splitter = new Mock<BLI.IFileSplitter>();
            splitter.Setup(s => s.SplitInputFileAsync()).ReturnsAsync(chunks);

            var merger = new Mock<BLI.IFileMerger>();
            var deleter = new Mock<BLI.IFileDeleter>();
            deleter.Setup(d => d.DeleteFilesAsync(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

            var result = await Create(folder.Path, splitter.Object, merger.Object, deleter.Object,
                                      consumerCount: 3, channelCapacity: 7).SortAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(2, result.Response!.TotalFiles);
            Assert.Equal(folder.File("sorted.txt"), result.Response.OutputFileName);
            Assert.Equal(3, result.Response.ConsumerCount);
            Assert.Equal(7, result.Response.ChannelCapacity);

            splitter.Verify(s => s.SplitInputFileAsync(), Times.Once);
            merger.Verify(m => m.MergeFiles(chunks), Times.Once);
            deleter.Verify(d => d.DeleteFilesAsync(chunks), Times.Once);
        }

        [Fact]
        public async Task SortAsync_DeletesPreExistingOutputFile()
        {
            using var folder = new TempFolder();

            var staleOutput = folder.File("sorted.txt");
            File.WriteAllText(staleOutput, "old output");

            var splitter = new Mock<BLI.IFileSplitter>();
            splitter.Setup(s => s.SplitInputFileAsync()).ReturnsAsync(new List<string>());

            var merger = new Mock<BLI.IFileMerger>();
            var deleter = new Mock<BLI.IFileDeleter>();
            deleter.Setup(d => d.DeleteFilesAsync(It.IsAny<List<string>>())).Returns(Task.CompletedTask);

            var result = await Create(folder.Path, splitter.Object, merger.Object, deleter.Object).SortAsync();

            Assert.True(result.IsSuccess);
            // merger is a no-op mock, so the stale file should have been removed and not recreated
            Assert.False(File.Exists(staleOutput));
        }

        [Fact]
        public async Task SortAsync_WhenSplitterThrows_ReturnsFailureAndSkipsMergeAndDelete()
        {
            using var folder = new TempFolder();

            var splitter = new Mock<BLI.IFileSplitter>();
            splitter.Setup(s => s.SplitInputFileAsync()).ThrowsAsync(new IOException("disk gone"));

            var merger = new Mock<BLI.IFileMerger>();
            var deleter = new Mock<BLI.IFileDeleter>();

            var result = await Create(folder.Path, splitter.Object, merger.Object, deleter.Object).SortAsync();

            Assert.False(result.IsSuccess);
            Assert.IsType<IOException>(result.Error);

            merger.Verify(m => m.MergeFiles(It.IsAny<List<string>>()), Times.Never);
            deleter.Verify(d => d.DeleteFilesAsync(It.IsAny<List<string>>()), Times.Never);
        }
    }
}
