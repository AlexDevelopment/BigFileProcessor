using Xunit;

using BLI = BusinessLogic.Services.Interfaces;
using Impl = BusinessLogic.Services.Implementations;

namespace Services.Tests
{
    public class RowContentProviderTests
    {
        private static BLI.IRowContentProvider Create(int[] numbers, string[] strings, int maxTextComponentCount)
        {
            return new Impl.RowContentProvider(
                OptionsFactory.Generator("ignored",
                                         numbers: numbers,
                                         strings: strings,
                                         maxTextComponentCount: maxTextComponentCount));
        }

        [Fact]
        public void Generate_ReturnsRowComposedFromConfiguredNumbersAndStrings()
        {
            var numbers = new[] { 5 };          // single number -> deterministic
            var strings = new[] { "alpha" };    // single token  -> deterministic content
            var provider = Create(numbers, strings, maxTextComponentCount: 2);

            var content = provider.Generate();

            Assert.False(content.IsEmpty);
            
            var contentString = content.ToString();
            var parts = contentString.Split('.');
            
            Assert.Equal(2, parts.Length);
            Assert.Equal("5", parts[0]);
            
            var textPart = parts[1].Trim();
            foreach (var token in textPart.Split(' '))
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
                var content = provider.Generate();

                Assert.False(content.IsEmpty);
                
                var contentString = content.ToString();
                var parts = contentString.Split('.');
                
                Assert.Equal(2, parts.Length);
                
                int number = int.Parse(parts[0]);
                Assert.True(number >= 1 && number <= 4);
                
                var textPart = parts[1].Trim();
                Assert.False(string.IsNullOrWhiteSpace(textPart));
            }
        }
    }
}

