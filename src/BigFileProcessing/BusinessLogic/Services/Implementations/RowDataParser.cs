using BusinessLogic.Exceptions;
using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    public class RowDataParser : BLI.IRowDataParser
    {
        #region Public Methods
        public BLO.RowData Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line) == true)
            {
                throw new RowDataParserException("input line cannot be null or whitespace.");
            }

            var parts = line.Split('.');

            if (parts.Length != 2)
            {
                throw new RowDataParserException($"input line must contain exactly one '.' character. Input line: '{line}'");
            }   

            if (int.TryParse(parts[0], out int number) == false)
            {
                throw new RowDataParserException($"input line must start with a valid integer. Input line: '{line}'");
            }

            if (string.IsNullOrWhiteSpace(parts[1]) == true)
            {
                throw new RowDataParserException($"input line must contain non-empty text after the '.' character. Input line: '{line}'");
            }

            return new BLO.RowData(Number: number, Text: parts[1]);
        }

        #endregion
    }
}
