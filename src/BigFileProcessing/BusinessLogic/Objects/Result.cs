

namespace BusinessLogic.Objects
{
    public class Result<T> where T : IServiceResponse
    {
        #region Private members

        private bool _isSuccess = false;
        private Exception? _error;
        private T? _response;

        #endregion


        #region Constructors

        private Result(T response)
        {
            _isSuccess = true;
            _error = null;
            _response = response;
        }

        private Result(Exception? error)
        {
            _isSuccess = false;
            _error = error;
            _response = default;
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
            return new Result<T>(response);
        }

        public static Result<T> Failure(Exception error)
        {
            return new Result<T>(error);
        }

        #endregion
    }
}
