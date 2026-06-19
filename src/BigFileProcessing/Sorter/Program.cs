using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using BSI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using BSIM = BusinessLogic.Services.Implementations;
using INF = Infrastructure;

using Sorter;

var services = new ServiceCollection();

services.AddFileSortingServices();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<BSI.IFileSorterService>();
var options = serviceProvider.GetRequiredService<IOptions<INF.SorterOptions>>();

Console.WriteLine($"input folder: {options.Value.Folder}\n\n");
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