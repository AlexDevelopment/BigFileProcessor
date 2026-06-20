
using Microsoft.Extensions.Logging;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    public class RowDataParser : BLI.IRowDataParser
    {
        #region Public Methods
        public BLO.RowData? Parse(string line)
        {
            if (string.IsNullOrEmpty(line) == true)
            {
                return null;
            }

            int dotIndex = line.IndexOf('.');
            if (dotIndex <= 0)
            {
                return null;
            }

            if (int.TryParse(line.AsSpan(0, dotIndex), out int number) == false)
            {
                return null;
            }

            int textStart = dotIndex + 2;

            if (textStart >= line.Length)
            {
                return null;
            }

            return new BLO.RowData(line, number, textStart);
        }

        #endregion
    }
}
