
using Services;

Console.WriteLine("start file generation...\n");

IFileGeneratorService service = new FileGeneratorService();

var request = new FileGenerationRequest();

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
