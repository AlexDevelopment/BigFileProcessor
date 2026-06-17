using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Objects
{
    public class Result<T> where T : IResponse
    {
        #region Private members

        private bool _isSuccess;
        private string _error;
        private T _response;

        #endregion


        #region Constructors

        private Result(bool isSuccess, string error = null, T response = default)
        {
            _isSuccess = isSuccess;
            _error = error;
            _response = response;
        }

        #endregion



        #region Public Properties

        public bool IsSuccess => _isSuccess;
        public string Error => _error;
        public T Response => _response;

        #endregion



        #region Public Methods

        public static Result<T> Success(T response)
        {
            return new Result<T>(true, response: response);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, error: error);
        }

        #endregion
    }
}
