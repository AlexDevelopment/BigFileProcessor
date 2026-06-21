using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public class ConfigureSorterOptionsDefaults : IConfigureOptions<SorterOptions>
    {
        #region Public Methods

        public void Configure(SorterOptions options)
        {
            if (options.MaxChunkSize <= 0)
            {
                options.MaxChunkSize = 1000; // Default max chunk size
            }

            if (options.ChannelCapacity <= 0)
            {
                options.ChannelCapacity = 2; // Default channel capacity
            }

            if (options.ConsumerCount <= 0)
            {
                options.ConsumerCount = 4; // Default to number of consumers
            }
        }

        #endregion
    }
}
