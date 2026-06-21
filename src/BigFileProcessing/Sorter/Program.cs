

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Sorter;

using BSI = BusinessLogic.Services.Interfaces;
using BLC = BusinessLogic.Constants;
using INF = Infrastructure;



var services = new ServiceCollection();

services.AddFileSortingServices();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileSorterService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.SorterOptions>>();

Console.WriteLine($"input folder: {options.Value.Folder}");
Console.WriteLine($"input file: {BLC.Files.InputFile}");
Console.WriteLine($"channel capacity: {options.Value.ChannelCapacity}");
Console.WriteLine($"consumer count: {options.Value.ConsumerCount}\n");
Console.WriteLine("start file sorting...\n");

var result = await service.SortAsync();

if (result.IsSuccess == true)
{
    Console.WriteLine($"file sorting completed. {result.Response?.ToLog()}");
}
else
{
    Console.WriteLine("file sorting has failed");
    Console.WriteLine($"error: {result.Error?.Message}");
}

Console.ReadLine();