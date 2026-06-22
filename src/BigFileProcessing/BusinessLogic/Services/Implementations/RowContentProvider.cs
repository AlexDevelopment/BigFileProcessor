using Microsoft.Extensions.Options;
using System.Text;
using BLO = BusinessLogic.Objects;
using BSI = BusinessLogic.Services.Interfaces;
using INF = Infrastructure;


namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Provides random content for rows in the system.
    /// </summary>
    public class RowContentProvider : BSI.IRowContentProvider
    {
        #region Private Members

        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;
        
        #endregion



        #region Constructor

        public RowContentProvider(IOptions<INF.GeneratorOptions> generatorOptions)
        {   
            _generatorOptions = generatorOptions;           
        }

        #endregion



        #region Public Methods
        public ReadOnlyMemory<char> Generate()
        {
            var strings = _generatorOptions.Value.Strings;
            var numbers = _generatorOptions.Value.Numbers;
            var maxTextComponentCount = _generatorOptions.Value.MaxTextComponentCount;

            int numbersIndex = Random.Shared.Next(0, numbers.Length);

            var builder = new StringBuilder();

            builder.Append(numbers[numbersIndex]);
            builder.Append(". ");

            int length = Random.Shared.Next(1, maxTextComponentCount + 1);

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    builder.Append(' ');
                }

                int stringIndex = Random.Shared.Next(strings.Length);
                builder.Append(strings[stringIndex]);
            }

            return builder.ToString().AsMemory();
        }

        #endregion
    }
}
