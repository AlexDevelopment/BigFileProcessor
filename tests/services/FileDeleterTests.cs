using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class FileDeleterTests
    {
        private readonly BLI.IFileDeleter _deleter = new Impl.FileDeleter();

        [Fact]
        public async Task DeleteFilesAsync_RemovesExistingFiles()
        {
            using var folder = new TempFolder();

            var a = folder.File("a.txt");
            var b = folder.File("b.txt");
            File.WriteAllText(a, "a");
            File.WriteAllText(b, "b");

            await _deleter.DeleteFilesAsync(new List<string> { a, b }, CancellationToken.None);

            Assert.False(File.Exists(a));
            Assert.False(File.Exists(b));
        }

        [Fact]
        public async Task DeleteFilesAsync_IgnoresMissingFiles()
        {
            using var folder = new TempFolder();

            var existing = folder.File("keep-then-delete.txt");
            File.WriteAllText(existing, "x");

            var missing = folder.File("does-not-exist.txt");

            // should not throw even though one of the files is absent
            await _deleter.DeleteFilesAsync(new List<string> { missing, existing }, CancellationToken.None);

            Assert.False(File.Exists(existing));
        }

        [Fact]
        public async Task DeleteFilesAsync_EmptyList_DoesNothing()
        {
            await _deleter.DeleteFilesAsync(new List<string>(), CancellationToken.None);
        }
    }
}
