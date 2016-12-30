// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTablePrinter.cs" company="Obscureware Solutions">
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
//   Defines the DataTablePrinter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Obscureware.Console.Operations.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ObscureWare.Console;

    /// <summary>
    /// Base class for printing content of the <see cref="DataTable{T}"/>. Contains base routines for measurement.
    /// </summary>
    public abstract class DataTablePrinter
    {
        private readonly IConsole _console;

        protected DataTablePrinter(IConsole console)
        {
            this._console = console;
        }

        /// <summary>
        /// Target, rendering console
        /// </summary>
        protected IConsole Console
        {
            get
            {
                return this._console;
            }
        }

        /// <summary>
        /// Prints data as simple, frame-less table
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public void PrintTable(ColumnInfo[] columns, string[][] rows)
        {
            this.CalculateRequiredRowSizes(columns, rows);

            int spacingWidth = columns.Length - 1;
            int totalAvailableWidth = this.Console.WindowWidth - 1; // -1 for ENDL - need to not overflow, to avoid empty lines
            // TODO: replace with write, cut on the end and 
            int maxRequiredWidth = columns.Select(col => col.CurrentLength).Sum() + spacingWidth + this.ExternalFrameThickness;

            // check if table fits to the screen width
            if (maxRequiredWidth > totalAvailableWidth)
            {
                int availableWidth = totalAvailableWidth - spacingWidth;
                float scale = (float)this.Console.WindowWidth / (float)maxRequiredWidth;

                for (int i = 0; i < columns.Length; i++)
                {
                    // TODO:probably round would be as good... gonna check
                    // TODO: OK, this works bad if the last column has min length - have to deduct all fixed columns from total and then divide content to the others...
                    int newLength = (int)Math.Floor(columns[i].CurrentLength * scale);
                    if (newLength < columns[i].MinLength)
                    {
                        newLength = Math.Min(availableWidth, columns[i].MinLength); // if all columns have minWidth and overflow the screen - some will not be displayed at all...
                        columns[i].CurrentLength = newLength;
                    }

                    availableWidth -= newLength;
                    if (availableWidth < 0)
                    {
                        newLength = newLength - Math.Abs(availableWidth);
                        availableWidth = 0;
                    }

                    columns[i].CurrentLength = Math.Max(0, newLength);
                }
            }

            // Now can render table
            this.RenderTable(columns, rows);
        }

        private void CalculateRequiredRowSizes(ColumnInfo[] columns, string[][] rows)
        {
            // headers room
            for (int i = 0; i < columns.Length; ++i)
            {
                columns[i].CurrentLength = Math.Max(columns[i].CurrentLength, columns[i].MinLength);

                foreach (string[] row in rows)
                {
                    if (i < row.Length) // some data might be missing, headers no
                    {
                        columns[i].CurrentLength = Math.Max(columns[i].CurrentLength, (row[i]?.Length) ?? 1);
                    }
                }
            }
        }

        private int[] CalculateRequiredRowSizes(string[] header, string[][] rows)
        {
            int[] result = new int[header.Length];

            // headers room
            for (int i = 0; i < header.Length; ++i)
            {
                result[i] = Math.Max(result[i], header[i].Length);

                foreach (string[] row in rows)
                {
                    if (i < row.Length) // some data might be missing, headers no
                    {
                        result[i] = Math.Max(result[i], (row[i]?.Length) ?? 1);
                    }
                }
            }

            return result;
        }

        public void PrintTable<T>(DataTable<T> table)
        {
            this.PrintTable(table.Columns.Values.ToArray(), table.GetRows().ToArray());
        }

        /// <summary>
        /// Specifies extra external frames total thickness
        /// </summary>
        protected abstract int ExternalFrameThickness { get; }

        /// <summary>
        /// Actual rendering function
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        protected abstract void RenderTable(ColumnInfo[] columns, IEnumerable<string[]> rows);
    }
}
