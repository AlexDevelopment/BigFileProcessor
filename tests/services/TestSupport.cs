using Microsoft.Extensions.Options;

using INF = Infrastructure;

namespace Services.Tests
{
    /// <summary>
    /// Creates a unique temporary directory and removes it (with all its content) on dispose.
    /// Used by the I/O bound services so every test runs against an isolated folder.
    /// </summary>
    internal sealed class TempFolder : IDisposable
    {
        public string Path { get; }

        public TempFolder()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "bfp_tests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string File(string name) => System.IO.Path.Combine(Path, name);

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path) == true)
                {
                    Directory.Delete(Path, recursive: true);
                }
            }
            catch
            {
                // best effort cleanup, never fail a test because of it
            }
        }
    }

    internal static class OptionsFactory
    {
        public static IOptions<INF.GeneratorOptions> Generator(string folder,
                                                               long maxFileSize = 1024,
                                                               int[]? numbers = null,
                                                               string[]? strings = null,
                                                               int maxTextComponentCount = 3)
        {
            return Options.Create(new INF.GeneratorOptions
            {
                Folder = folder,
                MaxFileSize = maxFileSize,
                Numbers = numbers ?? new[] { 1, 2, 3 },
                Strings = strings ?? new[] { "alpha", "beta", "gamma" },
                MaxTextComponentCount = maxTextComponentCount
            });
        }

        public static IOptions<INF.SorterOptions> Sorter(string folder,
                                                         long maxChunkSize = 1024,
                                                         int channelCapacity = 4,
                                                         int consumerCount = 2)
        {
            return Options.Create(new INF.SorterOptions
            {
                Folder = folder,
                MaxChunkSize = maxChunkSize,
                ChannelCapacity = channelCapacity,
                ConsumerCount = consumerCount
            });
        }
    }
}
