using BLO = BusinessLogic.Objects;

namespace BusinessLogic.Services.Interfaces
{
    public interface IRowContentProvider
    {
        BLO.RowData? Generate();
    }
}
