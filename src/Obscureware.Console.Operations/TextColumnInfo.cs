namespace Obscureware.Console.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages column definitions and parameters for displaying collections of objects in table (dynamically applying size of column width to be data/header dependent)
    /// </summary>
    internal class TextColumnInfo
    {
        private const string NULL_TEXT = "";
        private readonly string _header;

        public TextColumnInfo(string columnHeader, IEnumerable<string> values)
        {
            this._header = columnHeader;
            this.MaxLength = this.CalculateMaxLength(this._header, values.ToArray());
        }

        public void UpdateWithNewValues(IEnumerable<string> values)
        {
            this.MaxLength = this.CalculateMaxLength(this._header, values.ToArray());
        }

        public int MaxLength { get; private set; }

        public string GetDisplayHeader()
        {
            return this._header + new string(' ', this.MaxLength - this._header.Length);
        }

        public string GetDisplayValue(string value)
        {
            var text = GetDisplayText(value);
            return text + new string(' ', Math.Max(this.MaxLength - text.Length, 0));
        }

        private static string GetDisplayText(string value)
        {
            return value ?? NULL_TEXT;
        }

        /// <summary>
        /// Retrieves length of the longest string (or.ToString() result from provided objects and <paramref name="header"/>.
        /// </summary>
        /// <param name="header">Table column header text</param>
        /// <param name="values">All values from data rows in that column</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private int CalculateMaxLength(string header, params string[] values)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (values == null || !values.Any())
            {
                return header.Length;
            }

            return Math.Max(header.Length, values.Max(v => GetDisplayText(v).Length));
        }
    }
}