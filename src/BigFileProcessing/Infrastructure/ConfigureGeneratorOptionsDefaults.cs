using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public class ConfigureGeneratorOptionsDefaults : IConfigureOptions<GeneratorOptions>
    {
        #region Public Methods

        public void Configure(GeneratorOptions options)
        {

            if (options.Numbers == null || options.Numbers.Length == 0)
            {
                options.Numbers = new[] { 1, 2, 3, 4, 5 };
            }

            if (options.Strings == null || options.Strings.Length == 0)
            {
                options.Strings = new[] { "default", "value" };
            }

            if (options.MaxTextComponentCount <= 0)
            {
                options.MaxTextComponentCount = 5; // 5 as default
            }

            if (options.MaxFileSize <= 0)
            {
                options.MaxFileSize = 1048576; // 1 MB as default
            }
        }

        #endregion
    }
}
