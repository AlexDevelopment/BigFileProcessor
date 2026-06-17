using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using BSI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using BSIM = BusinessLogic.Services.Implementations;
using INF = Infrastructure;


var services = new ServiceCollection();

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

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileSorterService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.SorterOptions>>();

Console.WriteLine($"input folder: {options.Value.Folder}\n\n");
Console.WriteLine("start file sorting...\n");

var result = await service.SortAsync();

if (result.IsSuccess == true)
{
    Console.WriteLine("file sorting completed");
    Console.WriteLine($"elapsed time: {result.Response?.ElapsedTime} ms");
}
else
{
    Console.WriteLine("file sorting has failed");
    Console.WriteLine($"error: {result.Error?.Message}");
}

Console.ReadLine();