using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using INF = Infrastructure;

using BSI = BusinessLogic.Services.Interfaces;
using BSIM = BusinessLogic.Services.Implementations;


namespace Sorter
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSortingServices(
            this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            services
                .Configure<INF.SorterOptions>(
                    configuration.GetSection(INF.SorterOptions.SectionName))
                .ConfigureOptions<INF.ConfigureSorterOptionsDefaults>()
                .AddOptions<INF.SorterOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog("nlog.config");
            });

            services.AddSingleton<BSI.IFileSorterService, BSIM.FileSorterService>();
            services.AddSingleton<BSI.IFileSplitter, BSIM.FileSplitter>();
            services.AddSingleton<BSI.IFileMerger, BSIM.FileMerger>();
            services.AddSingleton<BSI.IFileDeleter, BSIM.FileDeleter>();
            services.AddSingleton<BSI.IRowDataParser, BSIM.RowDataParser>();
            services.AddSingleton<BSI.IChunkFileNameComposer, BSIM.ChunkFileNameComposer>();

            return services;
        }
    }
}
