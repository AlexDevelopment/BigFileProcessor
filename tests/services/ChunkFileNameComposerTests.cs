using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class ChunkFileNameComposerTests
    {
        private readonly BLI.IChunkFileNameComposer _composer = new Impl.ChunkFileNameComposer();

        [Theory]
        [InlineData(@"C:\data", 0, @"C:\data\chunk_0.txt")]
        [InlineData(@"C:\data", 7, @"C:\data\chunk_7.txt")]
        [InlineData(@"D:\tmp\sub", 123, @"D:\tmp\sub\chunk_123.txt")]
        public void GetFullFileName_ComposesFolderAndIndex(string folder, int index, string expected)
        {
            Assert.Equal(expected, _composer.GetFullFileName(folder, index));
        }

        [Fact]
        public void GetFullFileName_DifferentIndexes_ProduceDifferentNames()
        {
            var first = _composer.GetFullFileName(@"C:\data", 0);
            var second = _composer.GetFullFileName(@"C:\data", 1);

            Assert.NotEqual(first, second);
        }
    }
}
