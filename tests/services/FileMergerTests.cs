using BusinessLogic.Exceptions;
using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class FileMergerTests
    {
        private static BLI.IFileMerger Create(string folder)
        {
            return new Impl.FileMerger(OptionsFactory.Sorter(folder), new Impl.RowDataParser());
        }

        [Fact]
        public void MergeFiles_KWayMergesPreSortedFilesIntoGloballySortedOutput()
        {
            using var folder = new TempFolder();

            // each input file is already sorted by (text, number)
            var fileA = folder.File("chunk_0.txt");
            var fileB = folder.File("chunk_1.txt");
            File.WriteAllLines(fileA, new[] { "1. apple", "3. banana" });
            File.WriteAllLines(fileB, new[] { "5. apple", "2. cherry" });

            Create(folder.Path).MergeFiles(new List<string> { fileA, fileB });

            var output = File.ReadAllLines(folder.File("sorted.txt"));

            // apple(1) < apple(5) < banana(3) < cherry(2)
            Assert.Equal(new[] { "1. apple", "5. apple", "3. banana", "2. cherry" }, output);
        }

        [Fact]
        public void MergeFiles_SingleFile_OutputMatchesInput()
        {
            using var folder = new TempFolder();

            var file = folder.File("chunk_0.txt");
            File.WriteAllLines(file, new[] { "1. a", "2. b" });

            Create(folder.Path).MergeFiles(new List<string> { file });

            Assert.Equal(new[] { "1. a", "2. b" }, File.ReadAllLines(folder.File("sorted.txt")));
        }

        [Fact]
        public void MergeFiles_MissingInputFile_ThrowsFileMergerException()
        {
            using var folder = new TempFolder();

            var missing = folder.File("does-not-exist.txt");

            Assert.Throws<FileMergerException>(
                () => Create(folder.Path).MergeFiles(new List<string> { missing }));
        }
    }
}
