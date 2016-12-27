namespace Obscureware.Console.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    using ObscureWare.Console;

    using Tables;

    public class ConsoleOperations
    {
        private readonly IConsole _console;

        public ConsoleOperations(IConsole console)
        {
            this._console = console;
        }

        public bool WriteTextBox(Rectangle textArea, string text, FrameStyle frameDef)
        {
            int boxWidth = textArea.Width;
            int boxHeight = textArea.Height;
            this.LimitBoxDimensions(textArea.X, textArea.Y, ref boxWidth, ref boxHeight);
            Debug.Assert(boxWidth >= 3);
            Debug.Assert(boxHeight >= 3);
            this.WriteTextBoxFrame(textArea.X, textArea.Y, boxWidth, boxHeight, frameDef);
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
            return this.WriteTextBox(textArea.X, textArea.Y, textArea.Width, textArea.Height, text, colorDef);
        }

        public bool WriteTextBox(int x, int y, int boxWidth, int boxHeight, string text, ConsoleFontColor colorDef)
        {
            this.LimitBoxDimensions(x, y, ref boxWidth, ref boxHeight); // so do not have to check for this every line is drawn...
            this._console.SetCursorPosition(x, y);
            this._console.SetColors(colorDef.ForeColor, colorDef.BgColor);

            string[] lines = this.SplitText(text, boxWidth);
            int i;
            for (i = 0; i < lines.Length && i < boxHeight; ++i)
            {
                this._console.SetCursorPosition(x, y + i);
                this.WriteJustified(lines[i], boxWidth);
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



        private void WriteJustified(string text, int boxWidth)
        {
            if (text.Length == boxWidth)
            {
                Console.Write(text);
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
                    int cleanedLength = parts.Select(s => s.Length).Sum() + parts.Length - 1;
                    int remainingBlanks = boxWidth - cleanedLength;
                    if (remainingBlanks > cleanedLength / 2)
                    {
                        Console.Write(text); // text is way too short to expand it, keep to the left
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
                                string.Join(new string(' ', 2), parts.Take(remainingBlanks + 1)) +
                                string.Join(new string(' ', 1), parts.Skip(remainingBlanks + 1)));
                        }
                    }
                }
            }
        }

        private string[] SplitText(string text, int boxWidth)
        {
            // TODO: move it to some external toolset?
            // used this imperfect solution for now: http://stackoverflow.com/a/1678162
            // this will not work properly for long words
            // this is not able to properly break the words in the middle to optimize space...

            int offset = 0;
            var lines = new List<string>();
            while (offset < text.Length)
            {
                int index = text.LastIndexOf(" ", Math.Min(text.Length, offset + boxWidth)); // TODO: use CultureInfo!
                string line = text.Substring(offset, (index - offset <= 0 ? text.Length : index) - offset);
                offset += line.Length + 1;
                lines.Add(line);
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Limits box dimensions to actual window sizes (avoid overlapping AND exceptions...)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void LimitBoxDimensions(int x, int y, ref int width, ref int height)
        {
            if (x + width > this._console.WindowWidth)
            {
                width = this._console.WindowWidth - x;
            }
            if (y + height > this._console.WindowHeight)
            {
                height = this._console.WindowHeight - y;
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
