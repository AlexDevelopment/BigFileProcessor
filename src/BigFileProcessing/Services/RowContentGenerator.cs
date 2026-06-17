using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class RowContentGenerator : IRowContentGenerator
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
