
using Microsoft.Extensions.DependencyInjection;

using Services;

var services = new ServiceCollection();

services.AddSingleton<IFileGeneratorService, FileGeneratorService>();

var serviceProvider = services.BuildServiceProvider();

IFileGeneratorService service = serviceProvider.GetRequiredService<IFileGeneratorService>();

Console.WriteLine("start file generation...\n");

var request = new FileGenerationRequest() 
{ 
    NumberOfRecords = new Random().Next(1, 100)
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
