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

        private bool _isSuccess = false;
        private Exception? _error;
        private T? _response;

        #endregion


        #region Constructors

        private Result(bool isSuccess, Exception? error, T? response)
        {
            _isSuccess = isSuccess;
            _error = error;
            _response = response;
        }

        #endregion



        #region Public Properties

        public bool IsSuccess => _isSuccess;
        public Exception? Error => _error;
        public T? Response => _response;

        #endregion



        #region Public Methods

        public static Result<T> Success(T response)
        {
            return new Result<T>(true, error: null, response: response);
        }

        public static Result<T> Failure(Exception error)
        {
            return new Result<T>(false, error: error, response: default);
        }

        #endregion
    }
}
