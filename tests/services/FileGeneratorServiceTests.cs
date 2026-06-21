using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class FileGeneratorServiceTests
    {
        // "5. text" is 7 chars, the service counts rowBytes = length + 1 = 8.
        private static readonly ReadOnlyMemory<char> SampleRow = "5. text".AsMemory();

        private static BLI.IFileGeneratorService Create(string folder, long maxFileSize, BLI.IRowContentProvider provider)
        {
            return new Impl.FileGeneratorService(
                provider,
                OptionsFactory.Generator(folder, maxFileSize: maxFileSize),
                NullLogger<Impl.FileGeneratorService>.Instance);
        }

        [Fact]
        public async Task GenerateAsync_WritesRowsUntilMaxFileSizeReached()
        {
            using var folder = new TempFolder();

            var provider = new Mock<BLI.IRowContentProvider>();
            provider.Setup(p => p.Generate()).Returns(SampleRow);

            // 8 bytes per row, cap of 40 -> exactly 5 rows fit (5*8 = 40)
            var result = await Create(folder.Path, maxFileSize: 40, provider.Object).GenerateAsync(CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Response);
            Assert.Equal(5, result.Response!.TotalRecords);
            Assert.Equal(40, result.Response.SavedContentSize);

            var expectedFile = folder.File("unsorted.txt");
            Assert.Equal(expectedFile, result.Response.FileName);
            Assert.True(File.Exists(expectedFile));
            Assert.Equal(5, File.ReadAllLines(expectedFile).Length);
        }

        [Fact]
        public async Task GenerateAsync_OverwritesExistingInputFile()
        {
            using var folder = new TempFolder();

            var inputFile = folder.File("unsorted.txt");
            File.WriteAllText(inputFile, "stale content that must be replaced");

            var provider = new Mock<BLI.IRowContentProvider>();
            provider.Setup(p => p.Generate()).Returns(SampleRow);

            var result = await Create(folder.Path, maxFileSize: 16, provider.Object).GenerateAsync(CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.All(File.ReadAllLines(inputFile), line => Assert.Equal("5. text", line));
        }

        [Fact]
        public async Task GenerateAsync_WhenProviderThrows_ReturnsFailure()
        {
            using var folder = new TempFolder();

            var provider = new Mock<BLI.IRowContentProvider>();
            provider.Setup(p => p.Generate()).Throws(new InvalidOperationException("boom"));

            var result = await Create(folder.Path, maxFileSize: 40, provider.Object).GenerateAsync(CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.IsType<InvalidOperationException>(result.Error);
        }
    }
}

