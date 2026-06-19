using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using BSI =BusinessLogic.Services.Interfaces;
using INF = Infrastructure;

using Generator;


var services = new ServiceCollection();

services.AddFileGenerationServices();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileGeneratorService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.GeneratorOptions>>();

Console.WriteLine($"output folder: {options.Value.Folder}\n\n");
Console.WriteLine("start file generation...\n");

var result = await service.GenerateAsync();

if (result.IsSuccess == true)
{
    Console.WriteLine($"file generation completed, {result.Response?.ToLog()}");
}
else
{
    Console.WriteLine("file generation has failed");
    Console.WriteLine($"error: {result.Error}");
}

Console.ReadLine();