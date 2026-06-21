using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class RowDataParserTests
    {
        private readonly BLI.IRowDataParser _parser = new Impl.RowDataParser();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Parse_NullOrEmpty_ReturnsNull(string? line)
        {
            Assert.Null(_parser.Parse(line!));
        }

        [Theory]
        [InlineData("no dot here")]          // no '.' at all
        [InlineData(".text")]                // dot at index 0 -> dotIndex <= 0
        [InlineData("abc. text")]            // non numeric prefix
        [InlineData("12.")]                  // nothing after the dot
        [InlineData("12. ")]                 // textStart points past the last char
        public void Parse_MalformedLine_ReturnsNull(string line)
        {
            Assert.Null(_parser.Parse(line));
        }

        [Fact]
        public void Parse_ValidLine_ReturnsRowDataWithNumberAndText()
        {
            var row = _parser.Parse("42. hello world");

            Assert.NotNull(row);
            Assert.Equal("42. hello world", row!.Value.Original);
            Assert.Equal(42, row.Value.Number);
            Assert.Equal("hello world", row.Value.TextSpan.ToString());
        }

        [Fact]
        public void Parse_TextStart_SkipsDotAndSingleSpace()
        {
            // "7. a" -> dotIndex = 1, textStart = 3 -> "a"
            var row = _parser.Parse("7. a");

            Assert.NotNull(row);
            Assert.Equal(3, row!.Value.TextStart);
            Assert.Equal("a", row.Value.TextSpan.ToString());
        }
    }
}
