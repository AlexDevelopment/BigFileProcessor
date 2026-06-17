using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using BSI =BusinessLogic.Services.Interfaces;
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

services.AddOptions<INF.GeneratorOptions>()
    .Bind(configuration.GetSection(INF.GeneratorOptions.SectionName))
    .ValidateDataAnnotations();

services.AddSingleton<BSI.IFileGeneratorService, BSIM.FileGeneratorService>();
services.AddSingleton<BSI.IRowContentProvider, BSIM.SimpleRowContentProvider>();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileGeneratorService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.GeneratorOptions>>();

Console.WriteLine($"output folder: {options.Value.Folder}\n\n");
Console.WriteLine("start file generation...\n");

var result = await service.GenerateAsync();

if (result.IsSuccess == true)
{
    Console.WriteLine("file generation completed");
    Console.WriteLine($"file name: {result.Response.FileName}");
    Console.WriteLine($"number of records: {result.Response.TotalRecords:N0}");
    Console.WriteLine($"elapsed time: {result.Response.ElapsedTime:N0} ms");
    Console.WriteLine($"used memory: {result.Response.UsedMemory:N0} bytes");
    Console.WriteLine($"save content size: {result.Response.SavedContentSize/1024.0/1024.0:N0} MB");
}
else
{
    Console.WriteLine("file generation has failed");
    Console.WriteLine($"error: {result.Error}");
}

Console.ReadLine();