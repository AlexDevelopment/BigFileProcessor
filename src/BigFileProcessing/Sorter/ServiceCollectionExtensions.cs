using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddOptions<INF.SorterOptions>()
                .Bind(configuration.GetSection(INF.SorterOptions.SectionName))
                .ValidateDataAnnotations();

            services.AddSingleton<BSI.IFileSorterService, BSIM.FileSorterService>();
            services.AddSingleton<BSI.IFileSplitter, BSIM.FileSplitter>();
            services.AddSingleton<BSI.IRowDataParser, BSIM.RowDataParser>();

            return services;
        }
    }
}
