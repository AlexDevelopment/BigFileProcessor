using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FileGenerationResponse : IResponse
    {
        #region Private members

        private string _fileName;
        private int _numberOfRecords;

        #endregion



        #region Constructors
        public FileGenerationResponse(string fileName, int numberOfRecords)
        {
            _fileName = fileName;
            _numberOfRecords = numberOfRecords;
        }

        #endregion



        #region Public propeties 

        public string FileName => _fileName;
        public int NumberOfRecords => _numberOfRecords;

        #endregion
    }
}
