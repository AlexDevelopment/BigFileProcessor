

namespace BusinessLogic.Exceptions
{
    public class FileMergerException : Exception
    {
        public FileMergerException() : base() { }
        public FileMergerException(string message) 
                                    : base($"Error merging files: {message}") { }
        public FileMergerException(string message, Exception innerException) 
                                    : base($"Error merging files: {message}", innerException) { }
        public FileMergerException(Exception innerException)
                                    : base($"Error merging files: {innerException.Message}", innerException) { }
    }
}
