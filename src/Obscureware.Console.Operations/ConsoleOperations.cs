// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleOperations.cs" company="Obscureware Solutions">
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
//   Defines the ConsoleOperations class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ObscureWare.Console.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    using ObscureWare.Console;
    using ObscureWare.Shared;

    using Styles;
    using Tables;

    /// <summary>
    /// TODO: totally refactor this into pieces...
    /// </summary>
    public class ConsoleOperations
    {
        private readonly IConsole _console;

        public ConsoleOperations(IConsole console)
        {
            this._console = console;
        }

        public bool WriteTextBox(Rectangle textArea, string text, FrameStyle frameDef)
        {
            if (textArea.Width <= 0 || textArea.Height <= 0)
            {
                throw new ArgumentException("Rectangle must have reasonable ( >0 ) dimensions.", nameof(textArea));
            }

            uint boxWidth = (uint)textArea.Width;
            uint boxHeight = (uint)textArea.Height;
            this.LimitBoxDimensions(textArea.X, textArea.Y, ref boxWidth, ref boxHeight);
            Debug.Assert(boxWidth >= 3, "boxWidth >= 3");
            Debug.Assert(boxHeight >= 3, "boxHeight >= 3");
            this.WriteTextBoxFrame(textArea.X, textArea.Y, (int)boxWidth, (int)boxHeight, frameDef);
            return this.WriteTextBox(textArea.X + 1, textArea.Y + 1, boxWidth - 2, boxHeight - 2, text, frameDef.TextColor);
        }

        private void WriteTextBoxFrame(int boxX, int boxY, int boxWidth, int boxHeight, FrameStyle frameDef)
        {
            this._console.SetColors(frameDef.FrameColor.ForeColor, frameDef.FrameColor.BgColor);
            this._console.SetCursorPosition(boxX, boxY);
            this._console.WriteText(frameDef.TopLeft);
            for (int i = 1; i < boxWidth - 1; i++)
            {
                this._console.WriteText(frameDef.Top);
            }
            this._console.WriteText(frameDef.TopRight);
            string body = frameDef.Left + new string(frameDef.BackgroundFiller, boxWidth - 2) + frameDef.Right;
            for (int j = 1; j < boxHeight - 1; j++)
            {
                this._console.SetCursorPosition(boxX, boxY + j);
                this._console.WriteText(body);
            }
            this._console.SetCursorPosition(boxX, boxY + boxHeight - 1);
            this._console.WriteText(frameDef.BottomLeft);
            for (int i = 1; i < boxWidth - 1; i++)
            {
                this._console.WriteText(frameDef.Bottom);
            }
            this._console.WriteText(frameDef.BottomRight);
        }

        public bool WriteTextBox(Rectangle textArea, string text, ConsoleFontColor colorDef)
        {
            return this.WriteTextBox(textArea.X, textArea.Y, (uint) textArea.Width, (uint) textArea.Height, text, colorDef);
        }

        public bool WriteTextBox(int x, int y, uint boxWidth, uint boxHeight, string text, ConsoleFontColor colorDef)
        {
            this.LimitBoxDimensions(x, y, ref boxWidth, ref boxHeight); // so do not have to check for this every line is drawn...
            this._console.SetCursorPosition(x, y);
            this._console.SetColors(colorDef.ForeColor, colorDef.BgColor);

            string[] lines = text.SplitTextToFit(boxWidth).ToArray();
            int i;
            for (i = 0; i < lines.Length && i < boxHeight; ++i)
            {
                this._console.SetCursorPosition(x, y + i);
                this.WriteTextJustified(lines[i], boxWidth);
            }

            return i == lines.Length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="maxTableWidth">0 - to limit by windows size</param>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <param name="style"></param>
        public void WriteTabelaricData(int x, int y, int maxTableWidth, string[] headers, IEnumerable<string[]> values,
            TableStyle style)
        {
            this.LimitTableDimensions(x, ref maxTableWidth); // so do not have to check for this every line is drawn...
            this._console.SetCursorPosition(x, y);

            // table calculations - fitting content


            // display header

            // start writing rows... Perhaps try increasing buffer
            this.TabelericDisplay(headers, values.ToArray());

            //throw new NotImplementedException("later...");
        }

        /// <summary>
        /// Write text justified.
        /// </summary>
        /// <param name="text">The to be written.</param>
        /// <param name="boxWidth">Available area.</param>
        private void WriteTextJustified(string text, uint boxWidth)
        {
            if (text.Length == boxWidth)
            {
                Console.Write(text); // text that already spans whole box does not need justification
            }
            else
            {
                string[] parts = text.Split(new[] { @" ", @"\t" }, StringSplitOptions.RemoveEmptyEntries); // both split and clean
                if (parts.Length == 1)
                {
                    Console.Write(text); // we cannot do anything about one long word...
                }
                else
                {
                    uint cleanedLength = (uint)(parts.Select(s => s.Length).Sum() + parts.Length - 1);
                    uint remainingBlanks = boxWidth - cleanedLength;
                    if (remainingBlanks > cleanedLength / 2)
                    {
                        Console.Write(text); // text is way too short to expand it, keep it to the left
                    }
                    else
                    {
                        int longerSpacesCount = (int)Math.Floor((decimal)remainingBlanks / (parts.Length - 1));
                        if (longerSpacesCount > 1)
                        {
                            decimal remainingLowerSpacesJoins = remainingBlanks - (longerSpacesCount * (parts.Length - 1));
                            if (remainingLowerSpacesJoins > 0)
                            {
                                int longerQty = parts.Length - longerSpacesCount;
                                Console.Write(
                                    string.Join(new string(' ', longerSpacesCount), parts.Take(longerQty + 1)) +
                                    string.Join(new string(' ', longerSpacesCount - 1), parts.Skip(longerQty + 1)));
                            }
                            else
                            {
                                // all gaps equal
                                Console.Write(string.Join(new string(' ', longerSpacesCount), parts));
                            }
                        }
                        else
                        {
                            Console.Write(
                                string.Join(new string(' ', 2), parts.Take((int) (remainingBlanks + 1))) +
                                string.Join(new string(' ', 1), parts.Skip((int) (remainingBlanks + 1))));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Limits box dimensions to actual window sizes (avoid overlapping AND exceptions...)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void LimitBoxDimensions(int x, int y, ref uint width, ref uint height)
        {
            if (x + width > this._console.WindowWidth)
            {
                width = (uint)Math.Max(0, this._console.WindowWidth - x);
            }

            if (y + height > this._console.WindowHeight)
            {
                height = (uint)Math.Max(0, this._console.WindowHeight - y);
            }
        }

        private void LimitTableDimensions(int x, ref int maxTableWidth)
        {
            if (maxTableWidth == 0 || x + maxTableWidth > this._console.WindowWidth)
            {
                maxTableWidth = this._console.WindowWidth - x;
            }
        }



        protected void TabelericDisplay(string[] header, string[][] values)
        {
            var columns = BuildDisplayColumnInfo(header, values).ToArray();
            this.PrintTable(columns, values, rebuildColumnSizes: false);
            //if (realValues.Any())
            //{
            //    sb.AppendLine("While actual values are:");
            //    PrintTable(sb, columns, realValues, rebuildColumnSizes: false);
            //}
            //else
            //{
            //    sb.AppendLine("While there are no actual values...");
            //}

            //var s = sb.ToString();
            ////Debug.WriteLine(s);
            //return s;
        }

        private static IEnumerable<TextColumnInfo> BuildDisplayColumnInfo(string[] headerValues, string[][] values)
        {
            TextColumnInfo[] textColumns = new TextColumnInfo[headerValues.Length];
            int index = 0;
            foreach (var header in headerValues)
            {
                textColumns[index] = new TextColumnInfo(header, values.Select(row => row[index]).ToArray());
                index++;
            }

            return textColumns;
        }

        private void PrintTable(TextColumnInfo[] textColumns, string[][] rows,
            bool rebuildColumnSizes = true)
        {
            if (rebuildColumnSizes)
            {
                for (int i = 0; i < textColumns.Length; ++i)
                {
                    var columnInfo = textColumns[i];
                    columnInfo.UpdateWithNewValues(rows.Select(row => row[i]).ToArray());
                }
            }

            this._console.WriteText('|');

            //.AppendLine("\t| " + string.Join(" | ", textColumns.Select(col => col.GetDisplayHeader())) + " |");
            //foreach (var value in collection)
            //{
            //    sb.AppendLine("\t| " + string.Join(" | ", textColumns.Select(col => col.GetDisplayValue(value))) + " |");
            //}
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
                float scale = (float)this._console.WindowWidth / (float)maxRequiredWidth;
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
                // ReSharper disable once CoVariantArrayConversion, fine I want string version
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
                new []{ "A.Id" }.Concat(header).Select(head => new ColumnInfo(head)).ToArray());

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
