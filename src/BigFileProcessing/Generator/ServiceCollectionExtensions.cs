using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using INF = Infrastructure;

using BSI = BusinessLogic.Services.Interfaces;
using BSIM = BusinessLogic.Services.Implementations;


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

            services.AddOptions<INF.GeneratorOptions>()
                .Bind(configuration.GetSection(INF.GeneratorOptions.SectionName))
                .ValidateDataAnnotations();

            services.AddSingleton<BSI.IFileGeneratorService, BSIM.FileGeneratorService>();
            //services.AddSingleton<BSI.IRowContentProvider, BSIM.SimpleRowContentProvider>();
            services.AddSingleton<BSI.IRowContentProvider, BSIM.RowContentProvider>();

            return services;
        }
    }
}
