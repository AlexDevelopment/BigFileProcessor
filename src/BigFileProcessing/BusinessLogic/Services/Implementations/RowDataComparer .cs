using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    struct RowDataComparer : IComparer<BLO.RowData>
    {
        public int Compare(BLO.RowData x, BLO.RowData y)
        {
            int textCmp = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            return textCmp != 0 ? textCmp : x.Number.CompareTo(y.Number);
        }
    }
}
