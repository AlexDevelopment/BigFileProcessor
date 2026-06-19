

namespace BusinessLogic.Exceptions
{
    internal class RowDataParserException : Exception
    {
        public RowDataParserException() : base() { }
        public RowDataParserException(string message) 
                                    : base($"Error parsing row data: {message}") { }
        public RowDataParserException(string message, Exception innerException) 
                                    : base($"Error parsing row data: {message}", innerException) { }
        public RowDataParserException(Exception innerException)
                                    : base($"Error parsing row data: {innerException.Message}", innerException) { }
    }
}