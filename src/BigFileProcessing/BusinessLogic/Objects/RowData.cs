

namespace BusinessLogic.Objects
{
    public readonly struct RowData
    {
        public readonly string Original; 
        public readonly int Number;
        public readonly int TextStart;

        public RowData(string original, int number, int textStart)
        {
            Original = original;
            Number = number;
            TextStart = textStart;
        }

        public ReadOnlySpan<char> TextSpan => Original.AsSpan(TextStart);
    }
}
