using Microsoft.Extensions.Options;

using BSI = BusinessLogic.Services.Interfaces;

using INF = Infrastructure;

namespace BusinessLogic.Services.Implementations
{
    public class ExtendedRowContentProvider : BSI.IRowContentProvider
    {
        #region Private Members

        private readonly Random _random = new Random();
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;
        
        #endregion



        #region Constructor

        public ExtendedRowContentProvider(IOptions<INF.GeneratorOptions> generatorOptions)
        { 
            _generatorOptions = generatorOptions;
        }

        #endregion



        #region Public Methods
        public string Generate()
        {
            var strings = _generatorOptions.Value.Strings;
            var ints = _generatorOptions.Value.Numbers;

            int i = _random.Next(0, ints.Length - 1);
            int j = _random.Next(0, strings.Length - 1);

            return $"{ints[i]}.{strings[j]}";
        }

        #endregion
    }
}
