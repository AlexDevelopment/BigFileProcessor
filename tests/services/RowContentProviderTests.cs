using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class RowContentProviderTests
    {
        // RowContentProvider relies on the real parser to turn the composed string into RowData.
        private static BLI.IRowContentProvider Create(int[] numbers, string[] strings, int maxTextComponentCount)
        {
            return new Impl.RowContentProvider(
                OptionsFactory.Generator("ignored",
                                         numbers: numbers,
                                         strings: strings,
                                         maxTextComponentCount: maxTextComponentCount),
                new Impl.RowDataParser());
        }

        [Fact]
        public void Generate_ReturnsRowComposedFromConfiguredNumbersAndStrings()
        {
            var numbers = new[] { 5 };          // single number -> deterministic
            var strings = new[] { "alpha" };    // single token  -> deterministic content
            var provider = Create(numbers, strings, maxTextComponentCount: 2);

            var row = provider.Generate();

            Assert.NotNull(row);
            Assert.Equal(5, row!.Value.Number);

            foreach (var token in row.Value.TextSpan.ToString().Split(' '))
            {
                Assert.Contains(token, strings);
            }
        }

        [Fact]
        public void Generate_RepeatedCalls_AlwaysProduceParsableRows()
        {
            var provider = Create(new[] { 1, 2, 3, 4 },
                                  new[] { "one", "two", "three" },
                                  maxTextComponentCount: 4);

            for (int i = 0; i < 200; i++)
            {
                var row = provider.Generate();

                Assert.NotNull(row);
                Assert.True(row!.Value.Number >= 1 && row.Value.Number <= 4);
                Assert.False(string.IsNullOrWhiteSpace(row.Value.TextSpan.ToString()));
            }
        }
    }
}
