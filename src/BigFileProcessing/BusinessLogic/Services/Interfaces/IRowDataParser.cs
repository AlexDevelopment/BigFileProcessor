using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Interfaces
{
    public interface IRowDataParser
    {
        BLO.RowData? Parse(string line);
    }
}
