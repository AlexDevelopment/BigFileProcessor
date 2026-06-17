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
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

services.AddOptions<INF.GeneratorOptions>()
    .Bind(configuration.GetSection(INF.GeneratorOptions.SectionName))
    .ValidateDataAnnotations();

services.AddSingleton<BSI.IFileGeneratorService, BSIM.FileGeneratorService>();
services.AddSingleton<BSI.IFileContentProvider, BSIM.FileContentProvider>();
services.AddSingleton<BSI.IRowContentProvider, BSIM.SimpleRowContentProvider>();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileGeneratorService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.GeneratorOptions>>();

Console.WriteLine("start file generation...\n");
Console.WriteLine($"Output folder: {options.Value.Folder}\n");

var request = new BLO.FileGenerationRequest() 
{ 
    NumberOfRecords = 100000000
};

var result = await service.GenerateAsync(request);

if (result.IsSuccess == true)
{
    Console.WriteLine("file generation completed");
    Console.WriteLine($"file Name: {result.Response.FileName}");
    Console.WriteLine($"number of Records: {result.Response.NumberOfRecords}");
}
else
{
    Console.WriteLine("file generation has failed");
    Console.WriteLine($"error: {result.Error}");
}

Console.ReadLine();