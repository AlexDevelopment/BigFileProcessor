using System.Text;

using Microsoft.Extensions.Options;

using BLI = BusinessLogic.Services.Interfaces;
using BLC = BusinessLogic.Constants;
using BLO = BusinessLogic.Objects;

using INF = Infrastructure;



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
        public async Task<List<string>> SplitInputFileAsync()
        {
            int fileIndex = 0;
            long currentFileSize = 0;

            var fileContent = new List<BLO.RowData>();

            string chunkFileName = $"{_sorterOptions.Value.Folder}\\chunk_{fileIndex}.txt";

            var output = new List<string>() 
            {
                chunkFileName
            };

            StreamWriter writer = new StreamWriter(chunkFileName, false, Encoding.UTF8);

            using (var reader = new StreamReader($"{_sorterOptions.Value.Folder}\\{BLC.Files.InputFile}"))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine();

                    if (line == null) 
                    { 
                        continue; 
                    }

                    var row = _parser.Parse(line);

                    long rowSize = Encoding.UTF8.GetByteCount(row.ToString()) + 1;

                    if (currentFileSize + rowSize > _sorterOptions.Value.MaxChunkSize)
                    {                        
                        fileContent.Sort(new RowDataComparer());

                        foreach (var item in fileContent)
                        {
                            await writer.WriteLineAsync(item.ToString());
                        }                        

                        await writer.DisposeAsync();
                        writer.Close();

                        fileContent.Clear();
                        fileIndex++;
                        currentFileSize = 0;

                        chunkFileName = $"{_sorterOptions.Value.Folder}\\chunk_{fileIndex}.txt";

                        output.Add(chunkFileName);

                        writer = new StreamWriter(chunkFileName, false, Encoding.UTF8);
                    }

                    fileContent.Add(row);
                    currentFileSize += rowSize;
                }
                
                fileContent.Sort(new RowDataComparer());

                if (fileContent.Count > 0)
                {
                    foreach (var item in fileContent)
                    {
                        await writer.WriteLineAsync(item.ToString());
                    }
                }

                await writer.DisposeAsync();
                writer.Close();
            }

            return output;
        }

        #endregion
    }
}
