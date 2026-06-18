using BLO = BusinessLogic.Objects;
using BLI = BusinessLogic.Services.Interfaces;


namespace BusinessLogic.Services.Implementations
{
    public class RowDataParser : BLI.IRowDataParser
    {
        #region Public Methods
        public BLO.RowData Parse(string line)
        {
            var parts = line.Split('.');

            return new BLO.RowData(Number: int.Parse(parts[0]), Text: parts[1]);
        }

        #endregion
    }
}
