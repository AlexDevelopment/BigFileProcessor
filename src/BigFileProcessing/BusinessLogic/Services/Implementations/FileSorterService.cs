

using Microsoft.Extensions.Options;

using System.Diagnostics;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;
using BLC = BusinessLogic.Constants;
using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class FileSorterService : BLI.IFileSorterService
    {
        #region Private Members
        
        private readonly IOptions<INF.SorterOptions> _sorterOptions;

        #endregion



        #region Constructors
        public FileSorterService(IOptions<INF.SorterOptions> sorterOptions)
        {
            _sorterOptions = sorterOptions;
        }

        #endregion


        public async Task<BLO.Result<BLO.FileSortResponse>> SortAsync()
        {
            string inputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}";
            string outputFileName = $"{_sorterOptions.Value.Folder}\\{BLC.Files.OutputFile}";

            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();

                if (File.Exists(outputFileName) == true)
                {
                    File.Delete(outputFileName);
                }

                stopwatch.Stop();

                var output = new BLO.FileSortResponse()
                {
                    ElapsedTime = stopwatch.ElapsedMilliseconds
                };

                return BLO.Result<BLO.FileSortResponse>.Success(output);
            }
            catch (Exception ex)
            {
                return BLO.Result<BLO.FileSortResponse>.Failure(ex);
            }
        }
    }
}
