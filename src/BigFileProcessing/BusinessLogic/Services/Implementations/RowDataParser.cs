
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
                _logger.LogError("input line cannot be null or whitespace.");
                return null;
            }

            var parts = line.Split('.');

            if (parts.Length != 2)
            {
                _logger.LogError($"input line must contain exactly one '.' character. input line: '{line}'");
                return null;
            }

            if (int.TryParse(parts[0], out int number) == false)
            {
                _logger.LogError($"input line must start with a valid integer. input line: '{line}'");
                return null;
            }

            if (string.IsNullOrWhiteSpace(parts[1]) == true)
            {
                _logger.LogError($"input line must contain non-empty text after the '.' character. input line: '{line}'");
                return null;
            }

            return new BLO.RowData(Number: number, Text: parts[1]);
        }

        #endregion
    }
}
