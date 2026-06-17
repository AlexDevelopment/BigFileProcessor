using System;
using System.IO;
using System.Text;

namespace Services
{
    public class FileGeneratorService : IFileGeneratorService
    {
        #region Private Members

        private readonly IRowContentGenerator _rowContentGenerator;

        #endregion



        #region Constructors
        public FileGeneratorService(IRowContentGenerator rowContentGenerator)
        {
            _rowContentGenerator = rowContentGenerator;
        }

        #endregion



        #region Public Methods

        public Task<Result<FileGenerationResponse>> GenerateAsync(FileGenerationRequest request)
        {
            string fileName = $"not_sorted" +
                $".txt";

            using (var writer = new StreamWriter(fileName, 
                                                    append: false, 
                                                    encoding: Encoding.UTF8, 
                                                    bufferSize: 65536))
            {
                var rows = GenerateRows(request.NumberOfRecords);

                foreach (var row in rows)
                {
                    writer.WriteLine(row);
                }
            }

            var response = new FileGenerationResponse() 
            { 
                FileName = fileName,
                NumberOfRecords = request.NumberOfRecords
            };

            return Task.FromResult(Result<FileGenerationResponse>.Success(response));
        }

        #endregion



        #region Private Methods

        private List<string> GenerateRows(int numberOfRecords)
        {
            var rows = new List<string>();

            for (int i = 0; i < numberOfRecords; i++)
            {
                rows.Add(_rowContentGenerator.Generate());
            }

            return rows;
        }

        #endregion
    }
}
