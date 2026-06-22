

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using BSI = BusinessLogic.Services.Interfaces;
using BSIM = BusinessLogic.Services.Implementations;
using INF = Infrastructure;


namespace Generator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileGenerationServices(
            this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            services
                .Configure<INF.GeneratorOptions>(
                    configuration.GetSection(INF.GeneratorOptions.SectionName))
                .ConfigureOptions<INF.ConfigureGeneratorOptionsDefaults>()
                .AddOptions<INF.GeneratorOptions>()
                .ValidateDataAnnotations()
                .Validate(
                        options => !string.IsNullOrWhiteSpace(options.Folder),
                        "Folder must be specified.")
                .ValidateOnStart();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog("nlog.config");
            });

            services.AddSingleton<BSI.IFileGeneratorService, BSIM.FileGeneratorService>();
            services.AddSingleton<BSI.IRowContentProvider, BSIM.RowContentProvider>();
            services.AddSingleton<BSI.IRowDataParser, BSIM.RowDataParser>();

            return services;
        }
    }
}
