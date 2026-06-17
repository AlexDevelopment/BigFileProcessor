using BSI = BusinessLogic.Services.Interfaces;


namespace BusinessLogic.Services.Implementations
{
    public class SimpleRowContentProvider : BSI.IRowContentProvider
    {
        #region Private Members

        private readonly Random _random = new Random();

        #endregion



        #region Public Methods
        public string Generate()
        {
            return _random.NextInt64(0, 10000).ToString();
        }

        #endregion
    }
}
