

using BLE = BusinessLogic.Enums;

namespace BusinessLogic.Objects
{
    /// <summary>
    /// Represents the result of a service operation.
    /// </summary>
    /// <typeparam name="T">The type of the service response.</typeparam>
    public class Result<T> where T : IServiceResponse
    {
        #region Private members

        private BLE.ResultStates _state;
        private Exception? _error;
        private T? _response;

        #endregion


        #region Constructors

        private Result(T response)
        {
            _state = BLE.ResultStates.Success;
            _error = null;
            _response = response;
        }

        private Result(Exception error, bool isCancelled = false)
        {
            _state = isCancelled == true ? BLE.ResultStates.Cancelled : BLE.ResultStates.Failed;
            _error = error;
            _response = default;
        }

        #endregion



        #region Public Properties

        public bool IsSuccess => _state == BLE.ResultStates.Success;
        public BLE.ResultStates State => _state;
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
            return new Result<T>(error: error, isCancelled: false);
        }

        public static Result<T> Cancel(Exception error)
        {
            return new Result<T>(error: error, isCancelled: true);
        }

        #endregion
    }
}
