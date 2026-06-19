
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
            if (string.IsNullOrWhiteSpace(line) == true)
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

            string text = line[(dotIndex + 1)..];

            if (string.IsNullOrWhiteSpace(text) == true)
            {
                _logger.LogError("empty text part. input line: '{Line}'", line);
                return null;
            }

            return new BLO.RowData(Number: number, Text: text.Trim());
        }

        #endregion
    }
}
