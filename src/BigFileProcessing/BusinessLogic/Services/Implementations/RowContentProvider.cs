using Microsoft.Extensions.Options;

using BSI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;

using INF = Infrastructure;


namespace BusinessLogic.Services.Implementations
{
    public class RowContentProvider : BSI.IRowContentProvider
    {
        #region Private Members

        private readonly Random _random = new Random();
        private readonly IOptions<INF.GeneratorOptions> _generatorOptions;
        private readonly BSI.IRowDataParser _parser;
        
        #endregion



        #region Constructor

        public RowContentProvider(IOptions<INF.GeneratorOptions> generatorOptions,
                                    BSI.IRowDataParser parser)
        { 
            _generatorOptions = generatorOptions;
            _parser = parser;
        }

        #endregion



        #region Public Methods
        public BLO.RowData? Generate()
        {
            var strings = _generatorOptions.Value.Strings;
            var ints = _generatorOptions.Value.Numbers;
            var maxTextComponentCount = _generatorOptions.Value.MaxTextComponentCount;

            int i = _random.Next(0, ints.Length - 1);            

            int length = _random.Next(1, maxTextComponentCount);
            List<string> components = new List<string>();

            for (int k = 0; k < length; k++)
            {
                int index = _random.Next(0, strings.Length - 1);

                components.Add(strings[index]);
            }

            string text = string.Join(" ", components);

            return _parser.Parse($"{ints[i]}. {text}");
        }

        #endregion
    }
}
