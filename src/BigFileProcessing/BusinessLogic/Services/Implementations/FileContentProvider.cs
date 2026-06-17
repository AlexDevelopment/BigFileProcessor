
using BusinessLogic.Objects;
using Microsoft.Extensions.Options;
using BSI = BusinessLogic.Services.Interfaces;
using INF = Infrastructure;


namespace BusinessLogic.Services.Implementations
{
    public class FileContentProvider : BSI.IFileContentProvider
    {
        #region Private Members

        private readonly BSI.IRowContentProvider _rowContentProvider;
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;  

        #endregion



        #region Constructors
        public FileContentProvider(BSI.IRowContentProvider rowContentProvider, 
                                    IOptions<INF.GeneratorOptions> generatorOptions)
        {
            _rowContentProvider = rowContentProvider;
            _generatorOptions = generatorOptions;
        }

        #endregion



        #region Public Methods

        public FileContentGenerationResponse Generate()
        {
            var rows = new List<string>();

            for (int i = 0; i < _generatorOptions.Value.NumberOfRecords; i++)
            {
                var row = _rowContentProvider.Generate();

                rows.Add(row);
            }

            return new FileContentGenerationResponse
            {
                Content = string.Join(Environment.NewLine, rows),
                TotalRecords = rows.Count
            };
        }

        #endregion  
    }
}
