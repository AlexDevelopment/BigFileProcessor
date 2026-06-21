
using Microsoft.Extensions.Logging;

using BLI = BusinessLogic.Services.Interfaces;
using BLO = BusinessLogic.Objects;


namespace BusinessLogic.Services.Implementations
{
    public class RowDataParser : BLI.IRowDataParser
    {
        #region Public Methods

        /// <summary>
        /// Parses a line of text into a <see cref="BLO.RowData"/> object.
        /// <param name="line">The line of text to parse.</param>
        /// <returns>A <see cref="BLO.RowData"/> object if the line is valid; otherwise, null.</returns>
        /// <remarks>
        /// Validate the line format: "number. text"
        /// The number should be a positive integer, followed by a dot and a space, and then the text.
        /// Example of a valid line: "123. This is a sample text."
        /// Example of an invalid line: "abc. This is a sample text." or "123 This is a sample text."
        /// If the line is invalid, return null.
        /// The content is genetared by Generator console app which follows the same format, so we can assume that the input is valid.
        /// However, we will still validate it to be safe.
        /// </remarks>
        
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

            if (line[textStart - 1] != ' ')
            {
                return null;
            }

            return new BLO.RowData(line, number, textStart);
        }

        #endregion
    }
}
