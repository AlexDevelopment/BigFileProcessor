
using Microsoft.Extensions.Logging;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    public class RowDataParser : BLI.IRowDataParser
    {
        #region Private members

        private readonly ILogger<RowDataParser> _logger;

        #endregion



        #region Constructor
        public RowDataParser(ILogger<RowDataParser> logger)
        {
            _logger = logger;
        }

        #endregion



        #region Public Methods
        public BLO.RowData? Parse(string line)
        {
            if (string.IsNullOrEmpty(line) == true)
            {
                _logger.LogError("input line is null or whitespace.");
                return null;
            }

            int dotIndex = line.IndexOf('.');

            if (dotIndex <= 0)
            {
                _logger.LogError("invalid format. input line: '{Line}'", line);
                return null;
            }

            if (int.TryParse(line.AsSpan(0, dotIndex), out int number) == false)
            {
                _logger.LogError("input line does not start with a valid integer. input line: '{Line}'", line);
                return null;
            }

            // the explanation below

            string text = line[(dotIndex + 2)..];

            // done for optinization, to avoid creating a new string allocation for the trimmed text.
            // we can do it (dotIndex + 2) because incoming format is guaranteed to be "number. text" (with a space after the dot).

            if (string.IsNullOrEmpty(text) == true)
            {
                _logger.LogError("empty text part. input line: '{Line}'", line);
                return null;
            }

            return new BLO.RowData(Number: number, Text: text);
        }

        #endregion
    }
}
