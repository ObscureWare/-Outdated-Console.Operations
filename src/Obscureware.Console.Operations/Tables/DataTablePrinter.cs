namespace Obscureware.Console.Operations.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ObscureWare.Console;

    /// <summary>
    /// Base class for printing content of the <see cref="DataTable{T}"/>. Contains base routines for measurement.
    /// </summary>
    public  class DataTablePrinter // TODO: abstract and then simple table printer
    {
        private readonly IConsole _console;

        public DataTablePrinter(IConsole console)
        {
            this._console = console;
        }

        /// <summary>
        /// Prints data as simple, frame-less table
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="tableHeaderColor"></param>
        /// <param name="tableRowColor"></param>
        public void PrintAsSimpleTable(ColumnInfo[] columns, string[][] rows, ConsoleFontColor tableHeaderColor, ConsoleFontColor tableRowColor)
        {
            this.CalculateRequiredRowSizes(columns, rows);

            int spacingWidth = columns.Length - 1;
            int totalAvailableWidth = this._console.WindowWidth - 1; // -1 for ENDL - need to not overflow, to avoid empty lines
            int maxRequiredWidth = columns.Select(col => col.CurrentLength).Sum() + spacingWidth;
            if (maxRequiredWidth < totalAvailableWidth)
            {
                // cool, table fits to the screen
                int index = 0;
                string formatter = string.Join(" ", columns.Select(col => $"{{{index++},{col.CurrentLength * (int)col.Alignment}}}"));
                this._console.WriteLine(tableHeaderColor, string.Format(formatter, columns.Select(col => col.Header).ToArray()));
                foreach (string[] row in rows)
                {
                    // TODO: add missing cells...
                    this._console.WriteLine(tableRowColor, string.Format(formatter, row));
                }
            }
            else
            {
                int availableWidth = totalAvailableWidth - spacingWidth;
                float scale = (float) this._console.WindowWidth / (float)maxRequiredWidth;
                for (int i = 0; i < columns.Length; i++)
                {
                    // TODO:probably round would be as good... gonna check
                    int newLength = (int)Math.Floor(columns[i].CurrentLength * scale);
                    if (newLength < columns[i].MinLength)
                    {
                        newLength = Math.Min(availableWidth, columns[i].MinLength); // if all columns have minWidth and overflow the screen - some will not be displayed at all...
                        columns[i].CurrentLength = newLength;
                    }

                    availableWidth -= newLength;
                    if (availableWidth < 0)
                    {
                        newLength = newLength - (Math.Abs(availableWidth));
                        availableWidth = 0;
                    }
                    columns[i].CurrentLength = Math.Max(0, newLength);
                }

                int index = 0;
                string formatter = string.Join(" ", columns.Select(col => $"{{{index++},{(col.CurrentLength) * (int)col.Alignment}}}"));
                this._console.WriteLine(tableHeaderColor, string.Format(formatter, columns.Select(col => col.Header.Substring(0, Math.Min(col.Header.Length, col.CurrentLength))).ToArray()));
                foreach (string[] row in rows)
                {
                    string[] result = new string[columns.Length];
                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (row.Length > i) // taking care for assymetric array, btw
                        {
                            if (row[i].Length <= columns[i].CurrentLength)
                            {
                                result[i] = row[i];
                            }
                            else
                            {
                                result[i] = row[i].Substring(0, columns[i].CurrentLength);
                            }
                        }
                    }

                    this._console.WriteLine(tableRowColor, string.Format(formatter, result));
                }
            }
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

        public void PrintAsSimpleTable<T>(DataTable<T> table, ConsoleFontColor tableHeaderColor, ConsoleFontColor tableRowColor)
        {
            this.PrintAsSimpleTable(table.Columns.Values.ToArray(), table.GetRows().ToArray(), tableHeaderColor, tableRowColor);
        }

        public DataTable<T> BuildTable<T>(string[] header, IEnumerable<T> dataSource, Func<T, string[]> dataGenerator)
        {
            DataTable<T> table = new DataTable<T>(
                new[] { "A.Id" }.Concat(header).Select(head => new ColumnInfo(head)).ToArray());

            uint i = 1;
            foreach (T src in dataSource)
            {
                table.AddRow(src, new[] { i.ToAlphaEnum() + '.' }.Concat(dataGenerator.Invoke(src)).ToArray());
                i++;
            }

            return table;
        }
    }
}
