// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextColumnInfo.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2016 Sebastian Gruchacz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
// <summary>
//   Defines the TextColumnInfo class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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