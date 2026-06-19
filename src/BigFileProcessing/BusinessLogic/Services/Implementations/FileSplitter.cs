using System.Text;

using Microsoft.Extensions.Options;

using BLI = BusinessLogic.Services.Interfaces;
using BLC = BusinessLogic.Constants;
using BLO = BusinessLogic.Objects;

using INF = Infrastructure;

using BusinessLogic.Extensions;

namespace BusinessLogic.Services.Implementations
{
    public class FileSplitter : BLI.IFileSplitter
    {
        #region Private Members

        private readonly IOptions<INF.SorterOptions> _sorterOptions;
        private readonly BLI.IRowDataParser _parser;

        #endregion



        #region Constructors

        public FileSplitter(IOptions<INF.SorterOptions> sorterOptions, BLI.IRowDataParser parser)
        {
            _sorterOptions = sorterOptions;
            _parser = parser;
        }

        #endregion



        #region Public Methods
        public List<string> SplitInputFile()
        {
            int fileIndex = 0;
            long currentFileSize = 0;

            var fileContent = new List<BLO.RowData>();

            string chunkFileName = $"{_sorterOptions.Value.Folder}\\chunk_{fileIndex}.txt";

            var output = new List<string>() 
            {
                chunkFileName
            };

            StreamWriter writer = new StreamWriter(chunkFileName, false, Encoding.UTF8, 262144);

            using (var reader = new StreamReader($"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}", 
                                                                                    Encoding.UTF8, false, 262144))
            {
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    var row = _parser.Parse(line);

                    if (row == null)
                    {
                        continue;
                    }

                    BLO.RowData realRow = (BLO.RowData)row;

                    long rowSize = Encoding.UTF8.GetByteCount(realRow.Output) + 1;

                    if (currentFileSize + rowSize > _sorterOptions.Value.MaxChunkSize)
                    {                        
                        writer.SortWriteDispose(fileContent);

                        fileContent.Clear();
                        fileIndex++;
                        currentFileSize = 0;

                        chunkFileName = $"{_sorterOptions.Value.Folder}\\chunk_{fileIndex}.txt";

                        output.Add(chunkFileName);

                        writer = new StreamWriter(chunkFileName, false, Encoding.UTF8, 262144);
                    }

                    fileContent.Add(realRow);
                    currentFileSize += rowSize;
                }
                
                writer.SortWriteDispose(fileContent);
            }

            return output;
        }

        #endregion
    }
}
