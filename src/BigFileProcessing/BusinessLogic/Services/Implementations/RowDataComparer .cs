using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    /// <summary>
    /// Compares two RowData objects for sorting purposes.
    /// </summary>
    struct RowDataComparer : IComparer<BLO.RowData>
    {
        #region Public Methods

        public int Compare(BLO.RowData x, BLO.RowData y)
        {
            int cmp = x.TextSpan.CompareTo(y.TextSpan, StringComparison.Ordinal);

            return cmp != 0 ? cmp : x.Number.CompareTo(y.Number);
        }

        #endregion
    }
}
