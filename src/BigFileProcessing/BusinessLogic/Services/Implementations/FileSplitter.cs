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

        #endregion



        #region Constructors

        public FileSplitter(IOptions<INF.SorterOptions> sorterOptions)
        {
            _sorterOptions = sorterOptions;
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

                    var row = new BLO.RowData
                    (
                        Number: int.Parse(line.Split('.')[0]),
                        Text: line.Split('.')[1]
                    );

                    long rowSize = Encoding.UTF8.GetByteCount(row.ToString()) + 1;

                    if (currentFileSize + rowSize > _sorterOptions.Value.MaxChunkSize)
                    {
                        await writer.WriteAsync(string.Join(Environment.NewLine, fileContent.Select(r => r.ToString())));

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

                if (fileContent.Count > 0)
                { 
                    await writer.WriteAsync(string.Join(Environment.NewLine, fileContent.Select(r => r.ToString())));
                }

                await writer.DisposeAsync();
                writer.Close();
            }

            return output;
        }

        #endregion
    }
}
