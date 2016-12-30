// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTablePrinter.cs" company="Obscureware Solutions">
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
//   Defines the SimpleTablePrinter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Obscureware.Console.Operations.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ObscureWare.Console;

    public class SimpleTablePrinter : DataTablePrinter
    {
        private readonly ISimpleTableStyle style;

        public SimpleTablePrinter(IConsole console, ISimpleTableStyle style): base(console)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            this.style = style;
        }

        protected override int ExternalFrameThickness { get; } = 0; // no external frame

        protected override void RenderTable(ColumnInfo[] columns, IEnumerable<string[]> rows)
        {
            int index = 0;
            string formatter = string.Join(" ", columns.Select(col => $"{{{index++},{col.CurrentLength * (int)col.Alignment}}}"));

            //this.Console.WriteLine(this.style.TableHeaderColor, string.Format(formatter, columns.Select(col => col.Header).ToArray()));
            this.Console.WriteLine(this.style.TableHeaderColor, string.Format(formatter, columns.Select(col => col.Header.Substring(0, Math.Min(col.Header.Length, col.CurrentLength))).ToArray()));


            //foreach (string[] row in rows)
            //{
            //    // TODO: add missing cells...
            //    this.Console.WriteLine(this.style.TableRowColor, string.Format(formatter, row));
            //}
            
            foreach (string[] row in rows)
            {
                string[] result = new string[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    // taking care for asymmetric array, btw
                    if (row.Length > i)
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

                this.Console.WriteLine(this.style.TableRowColor, string.Format(formatter, result));
            }


            // TODO: implement and use Console.BatchPrint()! - atomic operation
        }
    }
}