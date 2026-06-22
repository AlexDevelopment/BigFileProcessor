using Microsoft.Extensions.Options;

using BSI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;

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
            var ints = _generatorOptions.Value.Numbers;
            var maxTextComponentCount = _generatorOptions.Value.MaxTextComponentCount;

            int intsIndex = Random.Shared.Next(0, ints.Length);            

            int length = Random.Shared.Next(1, maxTextComponentCount + 1);
            List<string> components = new List<string>();

            for (int k = 0; k < length; k++)
            {
                int index = Random.Shared.Next(0, strings.Length);

                components.Add(strings[index]);
            }

            string text = string.Join(" ", components);

            return $"{ints[intsIndex]}. {text}".AsMemory();
        }

        #endregion
    }
}
